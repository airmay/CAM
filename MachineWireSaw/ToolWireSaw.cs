using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Core;
using System;

namespace CAM
{
    /// <summary>
    /// Инструмент трос
    /// </summary>
    [Serializable]
    public class ToolWireSaw : ITool
    {
        public const int Length = 1500;

        /// <summary>
        /// Толщина троса
        /// </summary>
        public double Thickness { get; set; }

        public override string ToString() => Thickness.ToString();

        public Curve[] GetModel(Machine? machine)
        {
            var line = new Line(new Point3d(-Length, 0, 0), new Point3d(Length, 0, 0));
            return new Curve[]
            {
                line, 
                new Circle(line.StartPoint, Vector3d.XAxis, Thickness / 2),
                new Circle(line.EndPoint, Vector3d.XAxis, Thickness / 2),
                new Circle(line.GetPointAtParameter(line.Length / 2), Vector3d.XAxis, Thickness / 2)
            };
        }

        public Matrix3d GetTransformMatrix(ToolPosition from, ToolPosition to)
        {
            var displacement = Matrix3d.Displacement(from.Point.GetVectorTo(to.Point));
            var rotation = Matrix3d.Rotation(-(from.Angle - to.Angle), Vector3d.ZAxis, to.Point);

            return rotation * displacement;
        }
    }
}
