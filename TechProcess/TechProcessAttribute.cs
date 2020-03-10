using System;

namespace CAM
{
    public class TechProcessAttribute : Attribute
    {
        public int Number { get; }
        public string Caption { get; }

        public TechProcessAttribute(int number, string caption)
        {
            this.Number = number;
            this.Caption = caption;
        }
    }
}
