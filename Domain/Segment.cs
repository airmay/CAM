using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Обрабатываемый участок изделия
    /// </summary>
    public class Segment
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип обрабатываемого участка
        /// </summary>
        public SegmentType Type { get; set; }

        /// <summary>
        /// Точно начало
        /// </summary>
        public bool IsExactlyBegin { get; set; }

        /// <summary>
        /// Точно конец
        /// </summary>
        public bool IsExactlyEnd { get; set; }

        /// <summary>
        /// Параметры обработки
        /// </summary>
        ProcessingParams[] Params { get; set; }
    }
}
