using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;

namespace CAM.TechProcesses.CableSawing
{
    [Serializable]
    public abstract class CableSawingTechOperation : WireSawingTechOperation<CableSawingTechProcess>
    {
        public List<AcadObject> AcadObjects { get; set; }
        public int CuttingFeed { get; set; }
        public int S { get; set; }
        public double Approach { get; set; }
        public double Departure { get; set; }
        public double Delta { get; set; }

        public double Delay { get; set; }

        public bool IsRevereseDirection { get; set; }
        public bool IsRevereseOffset { get; set; }
        public virtual int StepCount { get; set; }

        public abstract Curve[] GetRailCurves(List<Curve> curves);
        //public abstract void BuildProcessing(CableCommandGenerator generator);

        public override void Init()
        {
            base.Init();
            TechProcess.SetOperationParams(this);
        }
    }
}
