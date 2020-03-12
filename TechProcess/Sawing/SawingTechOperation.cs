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

        public Side OuterSide { get; set; }

        public double AngleA { get; set; }

        public List<SawingMode> SawingModes { get; set; }

        public SawingTechOperation(ITechProcess techProcess, string caption) : base(techProcess, caption)
        {
        }

        public SawingTechOperation(ITechProcess techProcess, Border border) : base(techProcess, $"Распиловка{border.ObjectId.GetDesc()}")
        {
            SetFromBorder(border);
        }

        public void SetIsExactly(Corner corner, bool value)
        {
            if (corner == Corner.Start)
                IsExactlyBegin = value;
            else
                IsExactlyEnd = value;
        }

        public void SetFromBorder(Border border)
        {
            ProcessingArea = new AcadObject(border.ObjectId);
            var par = ((SawingTechProcess)TechProcess).SawingTechProcessParams;
            SawingModes = (border.ObjectId.IsLine() ? par.SawingLineModes : par.SawingCurveModes).ConvertAll(x => x.Clone());
            OuterSide = border.OuterSide;
            IsExactlyBegin = border.IsExactlyBegin;
            IsExactlyEnd = border.IsExactlyEnd;
        }

        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
    