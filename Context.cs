using CAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM
{
    public class Context
    {
        /// <summary>
        /// Текущий техпроцесс
        /// </summary>
        public TechProcess TechProcess { get; set; }

        /// <summary>
        /// Текущая техоперация
        /// </summary>
        public SawingTechOperation TechOperation { get; set; }
    }
}
