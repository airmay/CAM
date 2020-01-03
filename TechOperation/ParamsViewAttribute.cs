using System;

namespace CAM
{
    public class ParamsViewAttribute : Attribute
    {
        public ProcessingType ProcessingType { get; }

        public ParamsViewAttribute(ProcessingType type) => this.ProcessingType = type;
    }
}
