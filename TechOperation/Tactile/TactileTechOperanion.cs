using System;
using System.Collections.Generic;

namespace CAM.TechOperation.Tactile
{
    [Serializable]
    public class TactileTechOperanion : TechOperationBase
    {
        public override ProcessingType Type => ProcessingType.Tactile;

        public TactileParams TactileParams { get; }

        public override object Params => TactileParams;

        public TactileTechOperanion(TechProcess techProcess, ProcessingArea processingArea, TactileParams tactileParams)
            : base(techProcess, processingArea)
        {
            TactileParams = tactileParams;
        }

        public void CalcPassList()
        {
            var toolThickness = TechProcess.TechProcessParams.ToolThickness;
            var periodAll = TactileParams.GutterWidth - toolThickness;
            var periodWidth = toolThickness + toolThickness - 1;
            var count = Math.Ceiling(periodAll / periodWidth);
            periodWidth = periodAll / count;
            TactileParams.PassList = new List<Pass>();
            for (int i = 0; i < count; i++)
                TactileParams.PassList.Add(new Pass(i * periodWidth, CuttingType.Roughing));

            TactileParams.PassList.Add(new Pass(periodAll, CuttingType.Roughing));

            var x = toolThickness - (toolThickness - (periodWidth - toolThickness)) / 2;
            for (int i = 0; i < count; i++)
                TactileParams.PassList.Add(new Pass(i * periodWidth + x, CuttingType.Finishing));
        }

        public override List<CuttingParams> GetCuttingParams()
        {
            throw new NotImplementedException();
        }
    }
}
