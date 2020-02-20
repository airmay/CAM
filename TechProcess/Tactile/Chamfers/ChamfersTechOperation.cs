using System;

namespace CAM.Tactile
{
    [Serializable]
    [TechOperation(TechProcessNames.Tactile, "Фаска")]
    public class ChamfersTechOperation : TechOperationBase
    {
        public int ProcessingAngle { get; set; }

        public double BandStart { get; set; }

        public int Feed { get; set; }

        public ChamfersTechOperation(TactileTechProcess techProcess, string caption) : this(techProcess, caption, null, null) { }

        public ChamfersTechOperation(TactileTechProcess techProcess, string caption, int? processingAngle, double? bandStart) : base(techProcess, caption)
        {
            BandStart = bandStart ?? techProcess.BandStart1.Value;
            ProcessingAngle = processingAngle ?? techProcess.ProcessingAngle1.Value;
        }


        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
        {
        }
    }
}
