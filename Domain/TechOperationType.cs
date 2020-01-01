using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Вид технологической операции
    /// </summary>
    public enum TechOperationType
    {
        None,

        /// <summary>
        /// Распиловка
        /// </summary>
        Sawing,

        /// <summary>
        /// Профилирование
        /// </summary>
        Profiling
    }
}
