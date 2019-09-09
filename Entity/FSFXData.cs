using Hydrology.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public class FSFXData
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
        /// 瞬时风向
        /// </summary>
        public Nullable<Decimal> shfx { get; set; }
        /// <summary>
        /// 瞬时风速
        /// </summary>
        public Nullable<Decimal> shfs { get; set; }
        /// <summary>
        /// 一小时最大瞬时风向
        /// </summary>
        public Nullable<Decimal> yxszdshfx { get; set; }

        /// <summary>
        /// 一小时最大瞬时风速
        /// </summary>
        public Nullable<Decimal> yxszdshfs { get; set; }

        /// <summary>
        /// 最大瞬时风速时间
        /// </summary>
        public DateTime maxTime { get; set; }

        /// <summary>
        /// 2分钟平均风向
        /// </summary>
        public Nullable<Decimal> avg2fx { get; set; }

        /// <summary>
        /// 2分钟平均风速
        /// </summary>
        public Nullable<Decimal> avg2fs { get; set; }

        /// <summary>
        /// 10分钟平均风向
        /// </summary>
        public Nullable<Decimal> avg10fx { get; set; }

        /// <summary>
        /// 10分钟平均风速
        /// </summary>
        public Nullable<Decimal> avg10fs { get; set; }

        /// <summary>
        /// 10分钟最大风速
        /// </summary>
        public Nullable<Decimal> max10fs { get; set; }

        /// <summary>
        /// 10分钟最大风向
        /// </summary>
        public Nullable<Decimal> max10fx { get; set; }

        /// <summary>
        /// 10分钟最大风向时间
        /// </summary>
        public DateTime max10tm { get; set; }
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
