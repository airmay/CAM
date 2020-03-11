using System;
using System.Collections.Generic;
using System.Linq;

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

        public override List<ITechOperation> CreateTechOperations()
        {
            return ProcessingArea.ObjectIds.Select(p =>
            {
                var to = new SawingTechOperation(this);
                to.Caption = $"Распиловка{p.GetDesc()}{TechOperations.Count()}";
                to.ProcessingArea = new AcadObject(p);
                to.SawingModes = (p.IsLine() ? SawingTechProcessParams.SawingLineModes : SawingTechProcessParams.SawingCurveModes).ConvertAll(x => x.Clone());
                return (ITechOperation)to;
            })
            .ToList();
        }
    }
}
