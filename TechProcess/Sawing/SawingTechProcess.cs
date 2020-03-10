using System;

namespace CAM.Sawing
{
    [Serializable]
    [TechProcess(1, TechProcessNames.Sawing)]
    public class SawingTechProcess : TechProcessBase
    {
        public SawingTechProcessParams SawingTechProcessParams { get; }

        public double? Thickness { get; set; }

        public SawingTechProcess(string caption, Settings settings) : base(caption, settings)
        {
            SawingTechProcessParams = settings.SawingTechProcessParams.Clone();
        }
    }
}
