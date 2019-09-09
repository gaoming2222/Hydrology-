using System;
using System.Collections.Generic;
using Hydrology.Entity;

namespace Hydrology.Entity
{
    /// <summary>
    /// 查询的结果出来的事件
    /// </summary>
    public class CEventDBUIDataReadyArgs : EventArgs
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPageIndex { get; set; }

        /// <summary>
        /// 总共页码
        /// </summary>
        public int TotalPageCount { get; set; }

        /// <summary>
        /// 总共记录数
        /// </summary>
        public int TotalRowCount { get; set; }
    }

    /// <summary>
    /// 收到发送过来的单条数据的参数
    /// </summary>
    public class CEventRecvStationDataArgs : EventArgs
    {
        /// <summary>
        /// 测站ID,唯一编号
        /// </summary>
        public string StrStationID;

        /// <summary>
        /// 测站类型
        /// </summary>
        public EStationType EStationType;

        /// <summary>
        /// 消息类型
        /// </summary>
        public EMessageType EMessageType;

        /// <summary>
        /// 通讯方式类型
        /// </summary>
        public EChannelType EChannelType;

        /// <summary>
        /// 水位
        /// </summary>
        public Nullable<Decimal> WaterStage;

        /// <summary>
        /// 流量，仪器读取，需要计算，才能得到有用数值
        /// </summary>
        public Nullable<Decimal> TotalRain;

        /// <summary>
        /// 电压
        /// </summary>
        public Decimal Voltage;

        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime DataTime;

        /// <summary>
        /// 接收到数据的系统时间
        /// </summary>
        public DateTime RecvDataTime;

        /// <summary>
        /// 串口
        /// </summary>
        public string StrSerialPort;
    }

    /// <summary>
    /// 收到发送过来的多条数据的参数
    /// </summary>
    public class CEventRecvStationDatasArgs : EventArgs
    {
        
        /// <summary>
        /// 测站ID,唯一编号
        /// </summary>
        public string StrStationID;

        /// <summary>
        /// 测站类型
        /// </summary>
        public EStationType EStationType;

        /// <summary>
        /// 消息类型
        /// </summary>
        public EMessageType EMessageType;

        /// <summary>
        /// 通讯方式类型
        /// </summary>
        public EChannelType EChannelType;

        /// <summary>
        /// 数据值，最后一个的值为最新的值
        /// </summary>
        private List<CSingleStationData> m_listStationData;
        public List<CSingleStationData> Datas
        {
            get
            {
                if (m_listStationData == null)
                {
                    m_listStationData = new List<CSingleStationData>();
                }
                return m_listStationData;
            }
            set
            {
                // list 拷贝
                m_listStationData = new List<CSingleStationData>(value.ToArray());
            }
        }
        private List<CSingleStationObsData> m_listStationObsData;
        public List<CSingleStationObsData> obsDatas
        {
            get
            {
                if (m_listStationObsData == null)
                {
                    m_listStationObsData = new List<CSingleStationObsData>();
                }
                return m_listStationObsData;
            }
            set
            {
                // list 拷贝
                m_listStationObsData = new List<CSingleStationObsData>(value.ToArray());
            }
        }


        private List<CSingleStationEn2BData> m_listStationEn2BData;
        public List<CSingleStationEn2BData> en2bDatas
        {
            get
            {
                if (m_listStationEn2BData == null)
                {
                    m_listStationEn2BData = new List<CSingleStationEn2BData>();
                }
                return m_listStationEn2BData;
            }
            set
            {
                // list 拷贝
                m_listStationEn2BData = new List<CSingleStationEn2BData>(value.ToArray());
            }
        }

        /// <summary>
        /// 接收到数据的系统时间
        /// </summary>
        public DateTime RecvDataTime;

        /// <summary>
        /// 串口
        /// </summary>
        public string StrSerialPort;
    }

    public class CTextInfo
    {
        /// <summary>
        /// 信息时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 信息内容
        /// </summary>
        public String Info { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ETextMsgState EState { get; set; }
    }
    /// <summary>
    /// 单站单条数据记录
    /// </summary>
    public class CSingleStationData
    {
        /// <summary>
        /// 水位
        /// </summary>
        public Nullable<Decimal> WaterStage;

        /// <summary>
        /// 流量，仪器读取，需要计算，才能得到有用数值
        /// </summary>
        public Nullable<Decimal> TotalRain;

        /// <summary>
        /// 电压
        /// </summary>
        public Decimal? Voltage;

        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime DataTime;

        //流速相关*******************************************
        public Nullable<Decimal> Vm { get; set; }
        public Nullable<Decimal> W1 { get; set; }
        public Nullable<Decimal> Q { get; set; }
        public Nullable<Decimal> Q2 { get; set; }
        public Nullable<Decimal> v1 { get; set; }
        public Nullable<Decimal> v2 { get; set; }
        public Nullable<Decimal> v3 { get; set; }
        public Nullable<Decimal> v4 { get; set; }
        public Nullable<Decimal> v5 { get; set; }
        public Nullable<Decimal> v6 { get; set; }
        public Nullable<Decimal> v7 { get; set; }
        public Nullable<Decimal> v8 { get; set; }
        public Nullable<Decimal> beta1 { get; set; }
        public Nullable<Decimal> beta2 { get; set; }
        public Nullable<Decimal> beta3 { get; set; }
        public Nullable<Decimal> beta4 { get; set; }
        public Nullable<Decimal> W2 { get; set; }
        public string errorCode { get; set; }

        /// <summary>
        /// 蒸发
        /// </summary>
        public Nullable<Decimal> Eva;

        /// <summary>
        /// 温度
        /// </summary>
        public Nullable<Decimal> Temp;
        //****************************************************

        /// <summary>
        /// 状态
        /// </summary>
        public string EvpType;
    }
    public class CSingleStationEn2BData
    {
        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime dataTime;
        public Nullable<Decimal> water { get; set; }
        public Nullable<Decimal> shfx { get; set; }
        public Nullable<Decimal> shfs { get; set; }
        public Nullable<Decimal> yxszdshfx { get; set; }
        public Nullable<Decimal> yxszdshfs { get; set; }
        public DateTime maxTime { get; set; }
        public Nullable<Decimal> avg2fx { get; set; }
        public Nullable<Decimal> avg2fs { get; set; }
        public Nullable<Decimal> avg10fx { get; set; }
        public Nullable<Decimal> avg10fs { get; set; }
        public Nullable<Decimal> max10fx { get; set; }
        public Nullable<Decimal> max10fs { get; set; }
        public Nullable<Decimal> Visi1min { get; set; }
        public Nullable<Decimal> Visi10min { get; set; }
        public DateTime max10tm { get; set; }
        public Decimal? Voltage;
    }

    public class CSingleStationObsData
    {
        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime dataTime;
        /// <summary>
        /// 深度
        /// </summary>
        public Nullable<Decimal> Depth { get; set; }
        /// <summary>
        /// 浊度
        /// </summary>
        public Nullable<Decimal> Ntu { get; set; }
        /// <summary>
        /// 泥
        /// </summary>
        public Nullable<Decimal> Mud { get; set; }
        /// <summary>
        /// 温度
        /// </summary>
        public Nullable<Decimal> Tmp { get; set; }

        /// <summary>
        /// 电导率
        /// </summary>
        public Nullable<Decimal> Cndcty { get; set; }

        /// <summary>
        /// 盐度
        /// </summary>
        public Nullable<Decimal> Salinity { get; set; }

        /// <summary>
        /// 电压
        /// </summary>
        public Nullable<Decimal> Satt { get; set; }

        public Decimal? Voltage;

    }
        /// <summary>
        /// 串口状态
        /// </summary>
        public class CSerialPortState
    {
        /// <summary>
        /// 串口号
        /// </summary>
        public int PortNumber { get; set; }

        /// <summary>
        /// 是否正常
        /// </summary>
        public bool BNormal { get; set; }

        public EListeningProtType PortType { get; set; }
    }

    /// <summary>
    /// 单个数值的消息参数
    /// </summary>
    /// <typeparam name="T">实际参数类型</typeparam>
    public class CEventSingleArgs<T> : EventArgs
    {
        private T m_iValue;
        public CEventSingleArgs(T args)
            : base()
        {
            m_iValue = args;
        }
        public T Value
        {
            get { return m_iValue; }
        }
    }

    /// <summary>
    /// 当前墒情数据参数
    /// </summary>
    public class CSingleSoilDataArgs : EventArgs
    {
        /// <summary>
        /// 站点id, 需自己读数据库，将终端机号和站点ID进行映射匹配
        /// </summary>
        public string StrStationId { get; set; }

        /// <summary>
        /// 日期时间
        /// </summary>
        public DateTime DataTime { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public EMessageType EMessageType { get; set; }

        /// <summary>
        /// 通讯方式类型
        /// </summary>
        public EChannelType EChannelType { get; set; }

        ///电压值
        public Nullable<Decimal> Voltage { get; set; }

        /// <summary>
        /// 十厘米处的电压
        /// </summary>
        public Nullable<float> D10Value { get; set; }

        /// <summary>
        /// 二十厘米处的电压
        /// </summary>
        public Nullable<float> D20Value { get; set; }

        /// <summary>
        /// 三十厘米处的电压
        /// </summary>
        public Nullable<float> D30Value { get; set; }

        /// <summary>
        /// 四十厘米处的含水量
        /// </summary>
        public Nullable<float> D40Value { get; set; }

        /// <summary>
        /// 六十厘米处的电压
        /// </summary>
        public Nullable<float> D60Value { get; set; }
    }
}
