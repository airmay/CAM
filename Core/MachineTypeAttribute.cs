using System;

namespace CAM
{
    public class MachineTypeNewAttribute : Attribute
    {
        public MachineType MachineType { get; }

        public MachineTypeNewAttribute(MachineType machineType) => this.MachineType = machineType;
    }
}
