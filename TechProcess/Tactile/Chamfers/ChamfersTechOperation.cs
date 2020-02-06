using System;

namespace CAM.Tactile
{
    [Serializable]
    [TechOperation(TechProcessNames.Tactile, "Фаска")]
    public class ChamfersTechOperation : TechOperationBase
    {
        public int ProcessingAngle { get; set; }

        public int Feed { get; set; }

        public ChamfersTechOperation(TactileTechProcess techProcess, string name) : base(techProcess, name)
        {

        }

        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
        {
        }
    }
}
