using System;
using System.Collections.Generic;

namespace CAM.TechOperation.Profiling
{
    public class ProfilingTechOperation : TechOperationBase
    {
        public ProfilingTechOperation(TechProcess techProcess, ProcessingArea processingArea, string name) : base(techProcess, processingArea, name)
        {
        }

        public override ProcessingType Type => throw new NotImplementedException();

        public override object Params => throw new NotImplementedException();

        public override List<CuttingParams> GetCuttingParams()
        {
            throw new NotImplementedException();
        }
    }
}
