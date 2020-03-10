using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Sawing
{
    [Serializable]
    [TechProcess(1, TechProcessNames.Sawing)]
    public class SawingTechProcess : TechProcessBase
    {
        public SawingTechProcess(string caption, Settings settings) : base(caption, settings)
        {
        }
    }
}
