using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Базовая технологическая операция
    /// </summary>
    public abstract class TechOperationBase : ITechOperation
    {
        /// <summary>
        /// Параметры технологического процесса обработки
        /// </summary>
        public TechProcessParams TechProcessParams { get; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        protected TechOperationBase(TechProcessParams techProcessParams)
        {
            TechProcessParams = techProcessParams;
        }
    }
}
