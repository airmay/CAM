using System;

namespace CAM
{
    public class TechProcessAttribute : Attribute
    {
        public TechProcessType TechProcessType { get; }

        public TechProcessAttribute(TechProcessType type)
        {
            this.TechProcessType = type;
        }
    }
}
