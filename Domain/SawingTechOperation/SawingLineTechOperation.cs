using Autodesk.AutoCAD.DatabaseServices;
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
        /// Обрабатываемая область
        /// </summary>
        public LineProcessingArea processingArea { get; }

        public override ProcessingArea GetProcessingArea() => processingArea;

        public SawingLineTechOperation(TechProcessParams techProcessParams, SawingTechOperationParams techOperationParams, Line line)
             : base(techProcessParams, techOperationParams)
        {
            processingArea = new LineProcessingArea(line);
            Name = "Распил по прямой";
        }
    }
}
