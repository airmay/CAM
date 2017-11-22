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

        protected SawingTechOperation(TechProcess techProcess, ProcessingArea processingArea, SawingTechOperationParams techOperationParams)
            : base(techProcess, processingArea)
        {
            TechOperationParams = techOperationParams;
            Name = $"Распил-{ processingArea }";
        }
    }
}
