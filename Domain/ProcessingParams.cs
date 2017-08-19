using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Параметры обработки
    /// </summary>
    public class ProcessingParams
    {
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
    }
}
