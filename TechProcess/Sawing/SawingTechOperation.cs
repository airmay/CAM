using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Sawing
{
    [Serializable]
    [TechOperation(TechProcessNames.Sawing)]
    public class SawingTechOperation : TechOperationBase
    {
        public SawingTechOperation(ITechProcess techProcess, string caption) : base(techProcess, caption)
        {
        }

        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
