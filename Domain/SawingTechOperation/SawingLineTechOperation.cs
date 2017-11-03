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
        //public LineProcessingArea processingArea { get; }

        public SawingLineTechOperation(TechProcessParams techProcessParams, SawingTechOperationParams techOperationParams, LineProcessingArea processingArea)
             : base(techProcessParams, techOperationParams, processingArea)
        {
        }

        public override List<TechProcessCommand> GetProcessing()
        {
            var commands = new List<TechProcessCommand>();
            return commands;
        }
    }
}
