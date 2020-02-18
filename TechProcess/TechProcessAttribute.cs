using System;

namespace CAM
{
    public class TechProcessAttribute : Attribute
    {
        public string Caption { get; }
        public TechProcessAttribute(string caption)
        {
            this.Caption = caption;
        }
    }
}
