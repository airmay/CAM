using System;
using CAM.CncWorkCenter;

namespace CAM.Core
{
    public static class ProcessingFactory
    {
        public static IProcessing Create(MachineType machineType)
        {
            switch (machineType)
            {
                case MachineType.CncWorkCenter:
                    return new ProcessingCnc();
                case MachineType.WireSawMachine:
                    return new ProcessingCnc();
                default:
                    throw new ArgumentOutOfRangeException(nameof(machineType), machineType, null);
            }
        }
    }
}
