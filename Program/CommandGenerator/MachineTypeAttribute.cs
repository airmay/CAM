using System;

namespace CAM
{
    public class MachineTypeAttribute : Attribute
    {
        public Machine Machine { get; }

        public MachineTypeAttribute(Machine machine) => this.Machine = machine;
    }
}
