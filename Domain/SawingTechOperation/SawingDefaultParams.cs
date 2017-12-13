using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Параметры по-умолчанию технологической операция "Распиловка"
    /// </summary>
    public class SawingDefaultParams
    {

        /// <summary>
        /// Скорость заглубления
        /// </summary>
        public int PenetrationRate { get; set; } = 250;

        /// <summary>
        /// Режимы распиловки для прямой
        /// </summary>
        public List<SawingMode> LineModes { get; } = new List<SawingMode>();

        /// <summary>
        /// Режимы распиловки для дуги
        /// </summary>
        public List<SawingMode> ArcModes { get; } = new List<SawingMode>();

        //privare
        //public SawingTechOperationParams CreateLineParams()
        //{
        //    var sawingTechOperationParams = new SawingTechOperationParams();
        //    sawingTechOperationParams.Modes.AddRange(LineModes.ConvertAll(p => p.Clone()));
        //    return sawingTechOperationParams;
        //}

        //public SawingTechOperationParams CreateLineParams()
        //{
        //    var sawingTechOperationParams = new SawingTechOperationParams();
        //    sawingTechOperationParams.Modes.AddRange(LineModes.ConvertAll(p => p.Clone()));
        //    return sawingTechOperationParams;
        //}
    }
}
