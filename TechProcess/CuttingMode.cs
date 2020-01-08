using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM
{
    public class CuttingMode
    {
        /// <summary>
        /// Глубина
        /// </summary>
        public double Depth { get; set; }

        /// <summary>
        /// Шаг по глубине
        /// </summary>
        public double DepthStep { get; set; }

        /// <summary>
        /// Подача
        /// </summary>
        public int Feed { get; set; }

        //public SawingMode Clone() => (SawingMode)MemberwiseClone();
    }
}
