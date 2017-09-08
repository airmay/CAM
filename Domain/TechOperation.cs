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
    public abstract class TechOperation : ITechOperation
    {
        /// <summary>
        /// Параметры технологического процесса обработки
        /// </summary>
        public TechProcessParams TechProcessParams { get; }

        /// <summary>
        /// Получает обрабатываемую область
        /// </summary>
        /// <returns></returns>
        public abstract ProcessingArea GetProcessingArea();

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        protected TechOperation(TechProcessParams techProcessParams)
        {
            TechProcessParams = techProcessParams;
        }
    }
}
