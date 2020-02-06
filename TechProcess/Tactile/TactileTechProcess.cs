using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Tactile
{
    [Serializable]
    [TechProcess(TechProcessNames.Tactile)]
    public class TactileTechProcess: TechProcessBase
    {
        public TactileTechProcessParams TactileTechProcessParams { get; }
       
        public TactileTechProcess(string name, Settings settings) : base(name, settings)
        {
            TactileTechProcessParams = settings.TactileTechProcessParams.Clone();
            Material = Material.Granite;
        }
    }
}
