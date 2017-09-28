using System;
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
        /// Расчет параметра "Точно начало"
        /// </summary>
        public bool CalcExactlyBegin { get; set; } = true;

        /// <summary>
        /// Расчет параметра "Точно конец"
        /// </summary>
        public bool CalcExactlyEnd { get; set; } = true;


        public SawingTechOperationParams Clone()
        {
            var sawingTechOperationParams = new SawingTechOperationParams();
            sawingTechOperationParams.Modes.AddRange(Modes.ConvertAll(p => p.Clone()));
            return sawingTechOperationParams;
        }
    }
}
