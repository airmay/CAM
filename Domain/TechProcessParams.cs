using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Параметры технологического процесса обработки
    /// </summary>
    public class TechProcessParams
    {
        /// <summary>
        /// Высота подъема инструмента
        /// </summary>
        public int ZSafety { get; set; } = 20;

        //TODO public int MaterialType;

        /// <summary>
        /// Скорость заглубления
        /// </summary>
        public int PenetrationRate { get; set; }

        /// <summary>
        /// Частота вращения шпинделя
        /// </summary>
        public int Frequency { get; set; }
    }
}
