using System;
using CAM.CncWorkCenter;
using CAM.MachineWireSaw;

namespace CAM
{
    [Serializable]
    public abstract class OperationWireSawBase : OperationBase
    {
        protected ProcessingWireSaw Processing => (ProcessingWireSaw)ProcessingBase;
        protected ProcessorWireSaw Processor => Processing.Processor;

        public override MachineType MachineType => MachineType.WireSawMachine;
        public override ITool Tool => Processing.Tool;
    }
}
