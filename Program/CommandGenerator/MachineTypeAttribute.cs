using System;

namespace CAM
{
    public class MachineTypeAttribute : Attribute
    {
        public MachineCodes MachineCodes { get; }

        public MachineTypeAttribute(MachineCodes machineCodes) => this.MachineCodes = machineCodes;
    }
}
