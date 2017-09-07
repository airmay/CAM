using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Распиловка по дуге
    /// </summary>
    public class SawingArcTechOperation : SawingTechOperation
    {
        /// <summary>
        /// Режимы обработки
        /// </summary>
        List<SawingMode> Modes { get; } = new List<SawingMode>();


        /// <summary>
        /// Обрабатываемая область
        /// </summary>
        ArcProcessingArea processingArea { get; }
    }
}
