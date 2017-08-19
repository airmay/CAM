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

        #region Параметры обработки

        /// <summary>
        /// Точно начало
        /// </summary>
        public bool IsExactlyBegin { get; set; }

        /// <summary>
        /// Точно конец
        /// </summary>
        public bool IsExactlyEnd { get; set; }

        /// <summary>
        /// Глубина
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Шаг по глубине
        /// </summary>
        public int DepthStep { get; set; }

        /// <summary>
        /// Скорость подачи
        /// </summary>
        public int Speed { get; set; }

        #endregion
    }
}
