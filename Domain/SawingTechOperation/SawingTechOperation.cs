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
    public abstract class SawingTechOperation : TechOperationBase
    {
        /// <summary>
        /// Параметры технологической операции
        /// </summary>
        public SawingTechOperationParams TechOperationParams { get; }

        protected SawingTechOperation(TechProcessParams techProcessParams, SawingTechOperationParams techOperationParams)
            : base(techProcessParams)
        {
            TechOperationParams = techOperationParams;
        }
    }
}
