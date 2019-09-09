using DBManager.Interface;
using Entity;
using Hydrology.DBManager;
using Hydrology.DBManager.DB.SQLServer;
using Hydrology.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DBManager.DB.SQLServer
{
    public class CSQLObs : CSQLBase, IOBSProxy
    {
        #region 静态常量
        private const string CT_EntityName = "CEntityOBS";   //  数据库表Eva实体类
        public static readonly string CT_TableName = "OBSData";      //数据库中蒸发初始表的名字
        public static readonly string CN_StationId = "stationid";   //站点ID
        public static readonly string CN_DataTime = "datatime";    //数据的采集时间
        public static readonly string CN_Depth = "Depth";  
        public static readonly string CN_NTU = "NTU";  
        public static readonly string CN_ppm = "ppm";  
        public static readonly string CN_Temperature = "Temperature";  
        public static readonly string CN_Conductivity = "Conductivity";  
        public static readonly string CN_Salinity = "Salinity";  
        public static readonly string CN_Batt = "Batt"; 
        public static readonly string CN_transtype = "transtype";  
        public static readonly string CN_messagetype = "messagetype";  
        public static readonly string CN_recvdatatime = "recvdatatime"; 
        public static readonly string CN_State = "state";
        #endregion

        #region 成员变量
        private string m_strStaionId;       //需要查询的测站
        private DateTime m_startTime;  //查询起始时间
        private DateTime m_endTime;    //查询结束时间
        private bool m_TimeSelect;
        private string TimeSelectString
        {
            get
            {
                if (m_TimeSelect == false)
                {
                    return "";
                }
                else
                {
                    return "convert(VARCHAR," + CN_DataTime + ",120) LIKE '%00:00%' and ";
                }
            }
        }

        public System.Timers.Timer m_addTimer_1;
        #endregion

        public CSQLObs() : base()
        {
            // 为数据表添加列
            m_tableDataAdded.Columns.Add(CN_StationId);
            m_tableDataAdded.Columns.Add(CN_DataTime);
            m_tableDataAdded.Columns.Add(CN_Depth);
            m_tableDataAdded.Columns.Add(CN_NTU);
            m_tableDataAdded.Columns.Add(CN_ppm);
            m_tableDataAdded.Columns.Add(CN_Temperature);
            m_tableDataAdded.Columns.Add(CN_Conductivity);
            m_tableDataAdded.Columns.Add(CN_Salinity);
            m_tableDataAdded.Columns.Add(CN_Batt);
            m_tableDataAdded.Columns.Add(CN_transtype);
            m_tableDataAdded.Columns.Add(CN_messagetype);
            m_tableDataAdded.Columns.Add(CN_recvdatatime);
            m_tableDataAdded.Columns.Add(CN_State);

            m_mutexWriteToDB = CDBMutex.Mutex_TB_PWD;
            m_addTimer_1 = new System.Timers.Timer();
            m_addTimer_1.Elapsed += new System.Timers.ElapsedEventHandler(EHTimer_1);
            m_addTimer_1.Enabled = false;
            m_addTimer_1.Interval = CDBParams.GetInstance().AddToDbDelay;


        }
        protected virtual void EHTimer_1(object source, System.Timers.ElapsedEventArgs e)
        {
            //定时器事件，将所有的记录都写入数据库
            m_addTimer_1.Stop();  //停止定时器
            m_dateTimePreAddTime = DateTime.Now;
            //将数据写入数据库
            NewTask(() => { InsertSqlBulk(m_tableDataAdded); });
        }
        private void InsertSqlBulk(DataTable dt)
        {
            // 然后获取内存表的访问权
            m_mutexDataTable.WaitOne();

            if (dt.Rows.Count <= 0)
            {
                m_mutexDataTable.ReleaseMutex();
                return;
            }
            //清空内存表的所有内容，把内容复制到临日表tmp中
            DataTable tmp = dt.Copy();
            m_tableDataAdded.Rows.Clear();

            // 释放内存表的互斥量
            m_mutexDataTable.ReleaseMutex();

            try
            {
                //将临日表中的内容写入数据库
                string connstr = CDBManager.Instance.GetConnectionString();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connstr, SqlBulkCopyOptions.FireTriggers))
                {
                    // 蒸发表有插入触发器，如果遇到重复记录，则更新为当前的最新记录
                    //bulkCopy.BatchSize = 1;
                    bulkCopy.BulkCopyTimeout = 1800;
                    bulkCopy.DestinationTableName = CSQLObs.CT_TableName;
                    bulkCopy.ColumnMappings.Add(CN_StationId, CN_StationId);
                    bulkCopy.ColumnMappings.Add(CN_DataTime, CN_DataTime);
                    bulkCopy.ColumnMappings.Add(CN_Depth, CN_Depth);
                    bulkCopy.ColumnMappings.Add(CN_NTU, CN_NTU);
                    bulkCopy.ColumnMappings.Add(CN_ppm, CN_ppm);
                    bulkCopy.ColumnMappings.Add(CN_Temperature, CN_Temperature);
                    bulkCopy.ColumnMappings.Add(CN_Conductivity, CN_Conductivity);
                    bulkCopy.ColumnMappings.Add(CN_Salinity, CN_Salinity);
                    bulkCopy.ColumnMappings.Add(CN_Batt, CN_Batt);
                    bulkCopy.ColumnMappings.Add(CN_transtype, CN_transtype);
                    bulkCopy.ColumnMappings.Add(CN_messagetype, CN_messagetype);
                    bulkCopy.ColumnMappings.Add(CN_recvdatatime, CN_recvdatatime);
                    bulkCopy.ColumnMappings.Add(CN_State, CN_State);

                    try
                    {
                        bulkCopy.WriteToServer(tmp);
                        Debug.WriteLine("###{0} :add {1} lines to OBSDatas db", DateTime.Now, tmp.Rows.Count);
                        CDBLog.Instance.AddInfo(string.Format("添加{0}行到浊度数据表", tmp.Rows.Count));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                        //如果出现异常，SqlBulkCopy 会使数据库回滚，所有Table中的记录都不会插入到数据库中，
                        //此时，把Table折半插入，先插入一半，再插入一半。如此递归，直到只有一行时，如果插入异常，则返回。
                        if (tmp.Rows.Count == 1)
                            return;
                        int middle = tmp.Rows.Count / 2;
                        DataTable table = tmp.Clone();
                        for (int i = 0; i < middle; i++)
                            table.ImportRow(tmp.Rows[i]);

                        InsertSqlBulk(table);

                        table.Clear();
                        for (int i = middle; i < tmp.Rows.Count; i++)
                            table.ImportRow(tmp.Rows[i]);
                        InsertSqlBulk(table);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return;
            }

            return;
        }
        public void AddNewRow(CEntityOBS obsData)
        {
            throw new NotImplementedException();
        }

        public void AddNewRows(List<CEntityOBS> obsDatas)
        {
            m_mutexDataTable.WaitOne(); //等待互斥量
            foreach (CEntityOBS obs in obsDatas)
            {
                DataRow row = m_tableDataAdded.NewRow();
                row[CN_StationId] = obs.StationID;
                row[CN_DataTime] = obs.TimeCollect.ToString(CDBParams.GetInstance().DBDateTimeFormat);
                row[CN_Depth] = obs.Depth;
                row[CN_NTU] = obs.NTU;
                row[CN_ppm] = obs.ppm;
                row[CN_Temperature] = obs.Temperature;
                row[CN_Conductivity] = obs.Conductivity;
                row[CN_Salinity] = obs.Salinity;
                row[CN_Batt] = obs.Batt;      

                row[CN_transtype] = CEnumHelper.ChannelTypeToDBStr(obs.ChannelType);
                row[CN_messagetype] = CEnumHelper.MessageTypeToDBStr(obs.MessageType);

                row[CN_recvdatatime] = obs.TimeRecieved;
                row[CN_State] = obs.BState;
                m_tableDataAdded.Rows.Add(row);
            }
            NewTask(() => { InsertSqlBulk(m_tableDataAdded); });
            m_mutexDataTable.ReleaseMutex();
        }

        public int GetPageCount()
        {
            if (-1 == m_iPageCount)
            {
                DoCountQuery();
            }
            return m_iPageCount;
        }

        public List<CEntityOBS> GetPageData(int pageIndex, bool irefresh)
        {
            if (pageIndex <= 0 || m_startTime == null || m_endTime == null || m_strStaionId == null)
            {
                return new List<CEntityOBS>();
            }
            // 获取某一页的数据，判断所需页面是否在内存中有值
            int startIndex = (pageIndex - 1) * CDBParams.GetInstance().UIPageRowCount + 1;
            int key = (int)(startIndex / CDBParams.GetInstance().DBPageRowCount) + 1; //对应于数据库中的索引
            int startRow = startIndex - (key - 1) * CDBParams.GetInstance().DBPageRowCount - 1;
            Debug.WriteLine("startIndex;{0} key:{1} startRow:{2}", startIndex, key, startRow);
            // 判断MAP中是否有值
            if (m_mapDataTable.ContainsKey(key))
            {
                // 从内存中读取
                return CopyDataToList(key, startRow);
            }
            else
            {
                // 从数据库中查询
                int topcount = key * CDBParams.GetInstance().DBPageRowCount;
                int rowidmim = topcount - CDBParams.GetInstance().DBPageRowCount;
                string sql = " select * from ( " +
                    "select top " + topcount.ToString() + " row_number() over( order by " + CN_DataTime + " ) as " + CN_RowId + ",* " +
                    "from " + CT_TableName + " " +
                    "where " + CN_StationId + "=" + m_strStaionId.ToString() + " " +
                    "and " + TimeSelectString + CN_DataTime + " between " + DateTimeToDBStr(m_startTime) +
                    "and " + DateTimeToDBStr(m_endTime) +
                    ") as tmp1 " +
                    "where " + CN_RowId + ">" + rowidmim.ToString() +
                    " order by " + CN_DataTime + " DESC";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
                DataTable dataTableTmp = new DataTable();
                adapter.Fill(dataTableTmp);
                m_mapDataTable.Add(key, dataTableTmp);
                return CopyDataToList(key, startRow);
            }
        }

        public List<CEntityOBS> getPwdDataByTime(string stationid, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public int GetRowCount()
        {
            if (-1 == m_iPageCount)
            {
                DoCountQuery();
            }
            return m_iRowCount;
        }

        public void SetFilter(string stationId, DateTime timeStart, DateTime timeEnd, bool TimeSelect)
        {
            // 设置查询条件
            if (null == m_strStaionId)
            {
                // 第一次查询
                m_iRowCount = -1;
                m_iPageCount = -1;
                m_strStaionId = stationId;
                m_startTime = timeStart;
                m_endTime = timeEnd;
                m_TimeSelect = TimeSelect;
            }
            else
            {
                // 不是第一次查询
                if (stationId != m_strStaionId || timeStart != m_startTime || timeEnd != m_endTime || m_TimeSelect != TimeSelect)
                {
                    m_iRowCount = -1;
                    m_iPageCount = -1;
                    m_mapDataTable.Clear(); //清空上次查询缓存
                }
                m_strStaionId = stationId;
                m_startTime = timeStart;
                m_endTime = timeEnd;
                m_TimeSelect = TimeSelect;
            }
        }

        protected override bool AddDataToDB()
        {
            // 然后获取内存表的访问权
            m_mutexDataTable.WaitOne();

            if (m_tableDataAdded.Rows.Count <= 0)
            {
                m_mutexDataTable.ReleaseMutex();
                return true;
            }
            //清空内存表的所有内容，把内容复制到临日表tmp中
            DataTable tmp = m_tableDataAdded.Copy();
            m_tableDataAdded.Rows.Clear();

            // 释放内存表的互斥量
            m_mutexDataTable.ReleaseMutex();

            // 先获取对数据库的唯一访问权
            m_mutexWriteToDB.WaitOne();

            try
            {
                //将临日表中的内容写入数据库
                string connstr = CDBManager.Instance.GetConnectionString();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connstr, SqlBulkCopyOptions.FireTriggers))
                {
                    // 蒸发表有插入触发器，如果遇到重复记录，则更新为当前的最新记录
                    bulkCopy.BatchSize = 1;
                    bulkCopy.BulkCopyTimeout = 1800;

                    bulkCopy.DestinationTableName = CSQLHEva.CT_TableName;
                    bulkCopy.ColumnMappings.Add(CN_StationId, CN_StationId);
                    bulkCopy.ColumnMappings.Add(CN_DataTime, CN_DataTime);
                    bulkCopy.ColumnMappings.Add(CN_Depth, CN_Depth);
                    bulkCopy.ColumnMappings.Add(CN_NTU, CN_NTU);
                    bulkCopy.ColumnMappings.Add(CN_ppm, CN_ppm);
                    bulkCopy.ColumnMappings.Add(CN_Temperature, CN_Temperature);
                    bulkCopy.ColumnMappings.Add(CN_Conductivity, CN_Conductivity);
                    bulkCopy.ColumnMappings.Add(CN_Salinity, CN_Salinity);
                    bulkCopy.ColumnMappings.Add(CN_Batt, CN_Batt);
                    bulkCopy.ColumnMappings.Add(CN_transtype, CN_transtype);
                    bulkCopy.ColumnMappings.Add(CN_messagetype, CN_messagetype);
                    bulkCopy.ColumnMappings.Add(CN_recvdatatime, CN_recvdatatime);
                    bulkCopy.ColumnMappings.Add(CN_State, CN_State);

                    try
                    {
                        bulkCopy.WriteToServer(tmp);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }

                }
                //conn.Close();   //关闭连接
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                m_mutexWriteToDB.ReleaseMutex();
                return false;
            }
            Debug.WriteLine("###{0} :add {1} lines to OBS db", DateTime.Now, tmp.Rows.Count);
            CDBLog.Instance.AddInfo(string.Format("添加{0}行到浊度表", tmp.Rows.Count));
            m_mutexWriteToDB.ReleaseMutex();
            return true;
        }

        #region 帮助方法
        private void DoCountQuery()
        {
            string sql = "select count(*) count from " + CT_TableName + " " +
                "where " + CN_StationId + " = " + m_strStaionId + " " +
                "and " + TimeSelectString + CN_DataTime + "  between " + DateTimeToDBStr(m_startTime) +
                 "and " + DateTimeToDBStr(m_endTime);
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
                DataTable dataTableTmp = new DataTable();
                adapter.Fill(dataTableTmp);
                m_iRowCount = Int32.Parse((dataTableTmp.Rows[0])[0].ToString());
                m_iPageCount = (int)Math.Ceiling((double)m_iRowCount / CDBParams.GetInstance().UIPageRowCount); //向上取整
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }

        private List<CEntityOBS> CopyDataToList(int key, int startRow)
        {
            List<CEntityOBS> result = new List<CEntityOBS>();
            try
            {
                // 取最小值 ，保证不越界
                int endRow = Math.Min(m_mapDataTable[key].Rows.Count, startRow + CDBParams.GetInstance().UIPageRowCount);
                DataTable table = m_mapDataTable[key];
                for (; startRow < endRow; ++startRow)
                {
                    CEntityOBS obs = new CEntityOBS();
                    //  rain.RainID = long.Parse(table.Rows[startRow][CN_RainID].ToString());
                    obs.StationID = table.Rows[startRow][CN_StationId].ToString();
                    obs.TimeCollect = DateTime.Parse(table.Rows[startRow][CN_DataTime].ToString());
                    if (!table.Rows[startRow][CN_Depth].ToString().Equals(""))
                    {
                        obs.Depth = Decimal.Parse(table.Rows[startRow][CN_Depth].ToString());
                    }
                    if (!table.Rows[startRow][CN_NTU].ToString().Equals(""))
                    {
                        obs.NTU = Decimal.Parse(table.Rows[startRow][CN_NTU].ToString());
                    }
                    if (!table.Rows[startRow][CN_ppm].ToString().Equals(""))
                    {
                        obs.ppm = Decimal.Parse(table.Rows[startRow][CN_ppm].ToString());
                    }
                    if (!table.Rows[startRow][CN_Temperature].ToString().Equals(""))
                    {
                        obs.Temperature = Decimal.Parse(table.Rows[startRow][CN_Temperature].ToString());
                    }
                    //obs.maxTime = DateTime.Parse(table.Rows[startRow][CN_maxTime].ToString());
                    if (!table.Rows[startRow][CN_Conductivity].ToString().Equals(""))
                    {
                        obs.Conductivity = Decimal.Parse(table.Rows[startRow][CN_Conductivity].ToString());
                    }
                    if (!table.Rows[startRow][CN_Salinity].ToString().Equals(""))
                    {
                        obs.Salinity = Decimal.Parse(table.Rows[startRow][CN_Salinity].ToString());
                    }
                    if (!table.Rows[startRow][CN_Batt].ToString().Equals(""))
                    {
                        obs.Batt = Decimal.Parse(table.Rows[startRow][CN_Batt].ToString());
                    }
                    obs.BState = 1;
                    obs.ChannelType = CEnumHelper.DBStrToChannelType(table.Rows[startRow][CN_transtype].ToString());
                    obs.MessageType = CEnumHelper.DBStrToMessageType(table.Rows[startRow][CN_messagetype].ToString());
                    obs.TimeRecieved = DateTime.Parse(table.Rows[startRow][CN_recvdatatime].ToString());
                    result.Add(obs);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("" + e.ToString());
            }
            return result;
        }

        #endregion
    }
}
