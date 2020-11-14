using System;

namespace CAM
{
    public class MenuItemAttribute : Attribute
    {
        public int Position { get; }

        public string Name { get; }

        public MenuItemAttribute(string name, int position = 99)
        {
            this.Name = name ;
            this.Position = position;
        }
    }
}
