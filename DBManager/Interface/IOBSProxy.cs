using Entity;
using Hydrology.DBManager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.Interface
{
    public interface IOBSProxy : IMultiThread
    {
        void AddNewRow(CEntityOBS pwdData);

        void AddNewRows(List<CEntityOBS> pwdDatas);

        void SetFilter(string stationId, DateTime timeStart, DateTime timeEnd, bool TimeSelect);

        /// <summary>
        /// 获取当前选择条件下，总共页面数
        /// </summary>
        /// <returns>-1 表示查询失败</returns>
        int GetPageCount();

        /// <summary>
        /// 获取当前选择条件下，总共的行数
        /// </summary>
        /// <returns>-1 表示查询失败</returns>
        int GetRowCount();

        List<CEntityOBS> GetPageData(int pageIndex, bool irefresh);

        List<CEntityOBS> getPwdDataByTime(string stationid, DateTime start, DateTime end);
    }
}
