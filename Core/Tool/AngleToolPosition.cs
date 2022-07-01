using Autodesk.AutoCAD.Geometry;

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

        public void Set(Point3d? point, double? angleC, double? angleA)
        {
            Point = point ?? Point;
            AngleC = angleC ?? AngleC;
            AngleA = angleA ?? AngleA;
        }

        public override Matrix3d GetTransformMatrixFrom(ToolPosition toolPosition)
        {
            var millPosition = (MillToolPosition)toolPosition ?? new MillToolPosition();
            var mat1 = Matrix3d.Displacement(millPosition.Point.GetVectorTo(Point));
            var mat2 = Matrix3d.Rotation(Graph.ToRad(millPosition.AngleC - AngleC), Vector3d.ZAxis, Point);
            var mat3 = Matrix3d.Rotation(Graph.ToRad(AngleA - millPosition.AngleA), Vector3d.XAxis.RotateBy(Graph.ToRad(-AngleC), Vector3d.ZAxis), Point);

            //var mat1 = Matrix3d.Displacement(Point.GetVectorTo(millPosition.Point));
            //var mat2 = Matrix3d.Rotation(Graph.ToRad(AngleC - millPosition.AngleC), Vector3d.ZAxis, millPosition.Point);
            //var mat3 = Matrix3d.Rotation(Graph.ToRad(millPosition.AngleA - AngleA), Vector3d.XAxis.RotateBy(Graph.ToRad(-millPosition.AngleC), Vector3d.ZAxis), millPosition.Point);

            return mat3 * mat2 * mat1;
        }
    }
}
