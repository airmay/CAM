using System;
using System.Collections.Generic;

namespace CAM.Sawing
{
    [Serializable]
    [TechOperation(1, TechProcessNames.Sawing)]
    public class SawingTechOperation : TechOperationBase
    {
        public bool IsExactlyBegin { get; set; }

        public bool IsExactlyEnd { get; set; }

        public double AngleA { get; set; }

        public List<SawingMode> SawingModes { get; set; }

        public SawingTechOperation(ITechProcess techProcess, string caption = null) : base(techProcess, caption)
        {
        }

        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
    