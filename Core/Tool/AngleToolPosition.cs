using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace CAM
{
    public class MillToolPosition : ToolPosition
    {
        public double AngleC { get; set; }
        public double AngleA { get; set; }

        public MillToolPosition() { }

        public MillToolPosition(Point3d point, double angleC, double angleA) : base(point)
        {
            AngleC = angleC;
            AngleA = angleA;
        }

        public void Set(Point3d? point, double? x, double? y, double? z, double? angleC, double? angleA)
        {
            X = point?.X ?? x ?? X;
            Y = point?.Y ?? y ?? Y;
            Z = point?.Z ?? z ?? Z;
            AngleC = angleC ?? AngleC;
            AngleA = angleA ?? AngleA;
        }

        public override Matrix3d GetTransformMatrixFrom(ToolPosition toolPosition)
        {
            var millPosition = (MillToolPosition)toolPosition ?? new MillToolPosition(Point3d.Origin, 0, 0);
            var mat1 = Matrix3d.Displacement(millPosition.Point.GetVectorTo(Point));
            var mat2 = Matrix3d.Rotation(Graph.ToRad(millPosition.AngleC - AngleC), Vector3d.ZAxis, Point);
            var mat3 = Matrix3d.Rotation(Graph.ToRad(AngleA - millPosition.AngleA), Vector3d.XAxis.RotateBy(Graph.ToRad(-AngleC), Vector3d.ZAxis), Point);

            //var mat1 = Matrix3d.Displacement(Point.GetVectorTo(millPosition.Point));
            //var mat2 = Matrix3d.Rotation(Graph.ToRad(AngleC - millPosition.AngleC), Vector3d.ZAxis, millPosition.Point);
            //var mat3 = Matrix3d.Rotation(Graph.ToRad(millPosition.AngleA - AngleA), Vector3d.XAxis.RotateBy(Graph.ToRad(-millPosition.AngleC), Vector3d.ZAxis), millPosition.Point);

            return mat3 * mat2 * mat1;
        }

        public MillToolPosition Create(Point3d? point = null, double? x = null, double? y = null, double? z = null, double? angleC = null, double? angleA = null)
        {
            return new MillToolPosition
            {
                X = point?.X ?? x ?? X,
                Y = point?.Y ?? y ?? Y,
                Z = point?.Z ?? z ?? Z,
                AngleC = angleC ?? AngleC,
                AngleA = angleA ?? AngleA
            };
        }

        public Dictionary<string, double?> GetParams()
        {
            return new Dictionary<string, double?>
            {
                ["X"] = X?.Round(4),
                ["Y"] = Y?.Round(4),
                ["Z"] = Z?.Round(4),
                ["C"] = AngleC.Round(4),
                ["A"] = AngleA.Round(4),
            };
        }
    }
}
