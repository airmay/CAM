using System;

namespace CAM.TechOperations.Profiling
{
    public class ProfilingTechOperation : TechOperationBase
    {
        public ProfilingTechOperation(TechProcess techProcess, ProcessingArea processingArea) : base(techProcess, processingArea)
        {
        }

        public override ProcessingType Type => throw new NotImplementedException();

        public override object Params => throw new NotImplementedException();

        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
