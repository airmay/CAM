using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Инструмент
    /// </summary>
    [Serializable]
    public class Tool
    {
        /// <summary>
        /// Номер
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Диаметр
        /// </summary>
        public double Diameter { get; set; }

        /// <summary>
        /// Толщина
        /// </summary>
        public double Thickness { get; set; }

    }
}
