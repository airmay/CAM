using System;

namespace CAM
{
    public class DefaultParamsViewAttribute : Attribute
    {
        public ProcessingType ProcessingType { get; }

        public DefaultParamsViewAttribute(ProcessingType type) => this.ProcessingType = type;
    }
}
