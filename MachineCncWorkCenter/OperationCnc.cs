using System;
using CAM.CncWorkCenter;

namespace CAM
{
    [Serializable]
    public abstract class OperationCnc : OperationBase<ProcessingCnc, ProcessorCnc>
    {
        public Machine Machine => Processing.Machine.Value;
        public int CuttingFeed => Processing.CuttingFeed;
    }
}
