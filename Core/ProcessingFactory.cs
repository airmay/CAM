using System;
using CAM.CncWorkCenter;

namespace CAM
{
    public static class ProcessingFactory
    {
        public static IProcessing CreateProcessing(MachineType machineType)
        {
            switch (machineType)
            {
                case MachineType.CncWorkCenter:
                    return new ProcessingCnc();
                case MachineType.WireSawMachine:
                    return new ProcessingWireSaw();
                default:
                    throw new ArgumentOutOfRangeException(nameof(machineType), machineType, null);
            }
        }
    }
}