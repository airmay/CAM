using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Распиловка по прямой
    /// </summary>
    public class SawingLineTechOperation : SawingTechOperation
    {
        /// <summary>
        /// Режим обработки
        /// </summary>
        SawingMode Mode { get; } = new SawingMode();

        /// <summary>
        /// Обрабатываемая область
        /// </summary>
        LineProcessingArea processingArea { get; }
    }
}
