using System;

namespace CAM.Tactile
{
    [Serializable]
    [TechOperation(TechProcessNames.Tactile, "Конусы")]
    public class ConesTechOperation : TechOperationBase
    {
        public Tool Tool { get; set; }

        public int Frequency { get; set; }

        public ConesTechOperation(TactileTechProcess techProcess, string name) : base(techProcess, name)
        {

        }

        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
        {
        }
    }
}