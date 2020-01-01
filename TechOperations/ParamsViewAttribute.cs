using System;

namespace CAM.Domain
{
    public class ParamsViewAttribute : Attribute
    {
        public ProcessingType ProcessingType { get; }

        public ParamsViewAttribute(ProcessingType type) => this.ProcessingType = type;
    }
}
