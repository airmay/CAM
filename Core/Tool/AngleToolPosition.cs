using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public class MillToolPosition : ToolPosition
    {
        public double AngleC { get; set; }
        public double AngleA { get; set; }

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

        //public override Matrix3d GetTransformMatrix()//AngleToolPosition newToolPosition)
        //{
        //    //var mat1 = Matrix3d.Displacement(Point.GetVectorTo(newToolPosition.Point));
        //    //var mat2 = Matrix3d.Rotation(Graph.ToRad(AngleC - newToolPosition.AngleC), Vector3d.ZAxis, newToolPosition.Point);
        //    //var mat3 = Matrix3d.Rotation(Graph.ToRad(newToolPosition.AngleA - AngleA), Vector3d.XAxis.RotateBy(Graph.ToRad(-newToolPosition.AngleC), Vector3d.ZAxis), newToolPosition.Point);

        //    //return mat3 * mat2 * mat1;
        //    return new Matrix3d();
        //}
    }
}
