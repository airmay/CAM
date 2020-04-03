using System;

namespace CAM.Disk3D
{
    [Serializable]
    [TechOperation(1, TechProcessNames.Disk3D, "Гребенка")]
    public class CombTechOperation : TechOperationBase
    {
        public double StepPass { get; set; }

        public double StartPass { get; set; }

        public double Penetration { get; set; }

        public double Delta { get; set; }

        public double StepLong { get; set; }

        public int CuttingFeed { get; set; }

        public CombTechOperation(ITechProcess techProcess, string caption) : base(techProcess, caption)
        {
        }

        public override void BuildProcessing(ICommandGenerator generator)
        {
        }
    }
}
