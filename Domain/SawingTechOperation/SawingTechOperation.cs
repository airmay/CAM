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
    public abstract class SawingTechOperation : TechOperation
    {
        /// <summary>
        /// Параметры технологической операции
        /// </summary>
        public SawingTechOperationParams TechOperationParams { get; }

        protected SawingTechOperation(TechProcessParams techProcessParams, SawingTechOperationParams techOperationParams, ProcessingArea processingArea)
            : base(techProcessParams, processingArea)
        {
            TechOperationParams = techOperationParams;
            Name = $"Распил-{ processingArea }";
        }
    }
}
