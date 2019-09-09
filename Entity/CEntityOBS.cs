using Hydrology.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public class CEntityOBS
    {
        /// <summary>
        ///  测站中心的ID
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        ///  雨量值的采集时间
        /// </summary>
        public DateTime TimeCollect { get; set; }

        /// <summary>
        /// 深度
        /// </summary>
        public Nullable<Decimal> Depth { get; set; }

        /// <summary>
        /// 浊度
        /// </summary>
        public Nullable<Decimal> NTU { get; set; }
        /// <summary>
        /// 泥
        /// </summary>
        public Nullable<Decimal> ppm { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public Nullable<Decimal> Temperature { get; set; }

        /// <summary>
        /// 电导率
        /// </summary>
        public Nullable<Decimal> Conductivity { get; set; }

        /// <summary>
        /// 盐度
        /// </summary>
        public Nullable<Decimal> Salinity { get; set; }

        /// <summary>
        /// 电池
        /// </summary>
        public Nullable<Decimal> Batt { get; set; }


        /// <summary>
        /// 通讯方式类型
        /// </summary>
        public EChannelType ChannelType { get; set; }
        /// <summary>
        /// 报文类型
        /// </summary>
        public EMessageType MessageType { get; set; }


        /// <summary>
        /// 系统接收数据的时间
        /// </summary>
        public DateTime TimeRecieved { get; set; }

        /// 当前记录状态
        /// </summary>
        public int BState { get; set; }

    }
}
