using Autodesk.AutoCAD.DatabaseServices;
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
        /// Обрабатываемая область
        /// </summary>
        //public ArcProcessingArea processingArea { get; }

       
        public SawingArcTechOperation(TechProcessParams techProcessParams, SawingTechOperationParams techOperationParams, ArcProcessingArea processingArea)
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
