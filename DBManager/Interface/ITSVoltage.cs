﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;

namespace Hydrology.DBManager.Interface
{
    public interface ITSVoltage
    {
        /// <summary>
        /// 异步添加雨量记录
        /// </summary>
        /// <param name="rain"></param>
        void AddNewRow(CEntityTSVoltage voltage);

        /// <summary>
        /// 异步添加新的雨量记录
        /// </summary>
       // /// <param name="rains"></param>
        void AddNewRows(List<CEntityTSVoltage> voltages);

        void SetFilter(string stationId, DateTime timeStart, DateTime timeEnd, bool TimeSelect);

        List<CEntityTSVoltage> GetPageData(int pageIndex);
    }
}
