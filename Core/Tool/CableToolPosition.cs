using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public class CableToolPosition : ToolPosition
    {
        //public Point3d Center { get; set; }
        //public double AngleA { get; set; }

        public CableToolPosition(Point3d point, Point3d center, double angleA) : base(point)
        {
            //Center = center;
            //AngleA = angleA;

            var mat1 = Matrix3d.Displacement(Point.GetAsVector());
            var mat2 = Matrix3d.Rotation(angleA.ToRad(), Vector3d.ZAxis, center);
            Matrix = mat2 * mat1;
            InvMatrix = Matrix.Inverse();
        }

        public CableToolPosition() : this(Point3d.Origin, Point3d.Origin, 0)
        { }

            //public override Matrix3d GetTransformMatrix()
            //{
            //    //var mat1 = Matrix3d.Rotation(-AngleA.ToRad(), Vector3d.ZAxis, Center);
            //    //var mat2 = Matrix3d.Displacement(Point.GetVectorTo(newToolPosition.Point));
            //    //var mat3 = Matrix3d.Rotation(newToolPosition.AngleA.ToRad(), Vector3d.ZAxis, newToolPosition.Center);

            //    //return mat3 * mat2 * mat1;

            //    var mat1 = Matrix3d.Displacement(Point.GetAsVector());
            //    var mat2 = Matrix3d.Rotation(AngleA, Vector3d.ZAxis, Center);
            //    return mat2 * mat1;
            //}
        }
}
