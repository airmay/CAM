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
            var line = new Line(new Point3d(0, -1500, 0), new Point3d(0, 1500, 0));
            return new Curve[]
            {
                new Circle(line.StartPoint, Vector3d.YAxis, Thickness / 2),
                new Circle(line.EndPoint, Vector3d.YAxis, Thickness / 2),
                new Circle(line.GetPointAtParameter(line.Length / 2), Vector3d.YAxis, Thickness / 2)
            };
        }

        public Matrix3d GetTransformMatrix(ToolLocationParams? locationParamsFrom, ToolLocationParams locationParamsTo)
        {
            var from = new ToolLocationCnc(locationParamsFrom);
            var to = new ToolLocationCnc(locationParamsTo);

            var mat1 = Matrix3d.Displacement(from.Point.GetVectorTo(to.Point));
            var mat2 = Matrix3d.Rotation((from.AngleC - to.AngleC).ToRad(), Vector3d.ZAxis, to.Point);
            var mat3 = Matrix3d.Rotation((from.AngleA - to.AngleA).ToRad(), Vector3d.XAxis.RotateBy(-to.AngleC.ToRad(), Vector3d.ZAxis), to.Point);

            return mat3 * mat2 * mat1;
        }
    }
}
