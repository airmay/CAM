using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    ///  Тип примитива автокада представляющий область
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CurveTypeAttribute : Attribute
    {
        public Type Type;

        public CurveTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}
