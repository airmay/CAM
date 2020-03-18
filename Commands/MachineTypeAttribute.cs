using System;

namespace CAM
{
    public class MachineTypeAttribute : Attribute
    {
        public MachineType MachineType { get; }

        public MachineTypeAttribute(MachineType machineType) => this.MachineType = machineType;
    }
}
