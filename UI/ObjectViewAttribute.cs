using System;

namespace CAM
{
    public class ObjectViewAttribute : Attribute
    {
        public Type ObjectType { get; }

        public ObjectViewAttribute(Type objectType) => this.ObjectType = objectType;
    }
}
