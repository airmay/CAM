using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using CAM.Core;
using CAM.CncWorkCenter;

namespace CAM
{
    /// <summary>
    /// Инструмент трос
    /// </summary>
    [Serializable]
    public class ToolWireSaw : ITool
    {
        /// <summary>
        /// Толщина троса
        /// </summary>
        public double Thickness { get; set; }

        public override string ToString() => Thickness.ToString();

        public Curve[] GetModel(Machine? machine)
        {
            var line = new Line(new Point3d(-1500, 0, 0), new Point3d(1500, 0, 0));
            return new Curve[]
            {
                line, 
                new Circle(line.StartPoint, Vector3d.XAxis, Thickness / 2),
                new Circle(line.EndPoint, Vector3d.XAxis, Thickness / 2),
                new Circle(line.GetPointAtParameter(line.Length / 2), Vector3d.XAxis, Thickness / 2)
            };
        }

        public Matrix3d GetTransformMatrix(ToolLocationParams? locationParamsFrom, ToolLocationParams locationParamsTo)
        {
            var from = new ToolLocationWireSaw(locationParamsFrom);
            var to = new ToolLocationWireSaw(locationParamsTo);

            var mat1 = Matrix3d.Displacement(from.Point.GetVectorTo(to.Point));
            var mat2 = Matrix3d.Rotation(-(from.Angle - to.Angle), Vector3d.ZAxis, to.Point);

            return mat2 * mat1;
        }
    }
}
