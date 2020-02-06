using System;

namespace CAM
{
    public class TechProcessAttribute : Attribute
    {
        public string Name { get; }
        public TechProcessAttribute(string name)
        {
            this.Name = name;
        }
    }
}
