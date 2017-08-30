using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Технологическая операция "Распиловка"
    /// </summary>
    public class SawingTechOperation : TechOperationBase
    {
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
        public bool CalcExactlyBegin { get; set; }

        /// <summary>
        /// Расчет параметра "Точно конец"
        /// </summary>
        public bool CalcExactlyEnd { get; set; }
    }
}
