﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Параметры технологической операция "Распиловка"
    /// </summary>
    public class SawingTechOperationParams
    {
        /// <summary>
        /// Скорость заглубления
        /// </summary>
        public int PenetrationRate { get; set; } = 250;

        /// <summary>
        /// Режимы распиловки
        /// </summary>
        public List<SawingMode> Modes { get; } = new List<SawingMode>();

        /// <summary>
        /// Точно начало
        /// </summary>
        public bool IsExactlyBegin { get; set; }

        /// <summary>
        /// Точно конец
        /// </summary>
        public bool IsExactlyEnd { get; set; }

        /// <summary>
        /// Компенсация
        /// </summary>
        public double Compensation { get; set; }

        public SawingTechOperationParams Clone()
        {
            var sawingTechOperationParams = new SawingTechOperationParams();
            sawingTechOperationParams.Modes.AddRange(Modes.ConvertAll(p => p.Clone()));
            return sawingTechOperationParams;
        }
    }
}
