using System;
using CAM.CncWorkCenter;

namespace CAM
{
    [Serializable]
    public abstract class OperationWireSawBase : OperationBase
    {
        protected ProcessingWireSaw Processing => (ProcessingWireSaw)ProcessingBase;
        protected ProcessorWireSaw Processor => Processing.Processor;

        public override MachineType MachineType => MachineType.CncWorkCenter;
        public override Machine Machine => Processing.Machine.Value;
        public override ITool Tool => Processing.Tool;
        public double ToolThickness => Processing.Tool.Thickness;
        public int CuttingFeed => Processing.CuttingFeed;

    }
}
