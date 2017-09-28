using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Режим распиловки
    /// </summary>
    public class SawingMode
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
        /// Подача
        /// </summary>
        public int Feed { get; set; }

        public SawingMode Clone()
        {
            return new SawingMode
            {
                Depth = this.Depth,
                DepthStep = this.DepthStep,
                Feed = this.Feed
            };
        }
    }
}
