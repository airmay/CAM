using System;
using CAM.CncWorkCenter;

namespace CAM
{
    [Serializable]
    public abstract class OperationCnc : OperationBase
    {
        protected ProcessingCnc Processing => (ProcessingCnc)ProcessingBase;
        protected ProcessorCnc Processor => Processing.Processor;

        public override MachineType MachineType => MachineType.CncWorkCenter;
        public Machine Machine => Processing.Machine.Value;
        public int CuttingFeed => Processing.CuttingFeed;
    }
}
