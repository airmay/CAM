using System;

namespace CAM.TechOperation
{
    public class ProcessingAttribute : Attribute
    {
        public ProcessingType Type { get; }

        public MachineType Machine { get; }

        public string Name { get; }

        public ProcessingAttribute(ProcessingType type, MachineType machine, string name)
        {
            this.Type = type;
            this.Machine = machine;
            this.Name = name;
        }
    }
}