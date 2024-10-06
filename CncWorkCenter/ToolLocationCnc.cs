using Autodesk.AutoCAD.Geometry;

namespace CAM.CncWorkCenter
{
    public class ToolLocationCnc : IToolLocation
    {
        public Point3d Point
        {
            get => new Point3d(X, Y, Z);
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        public double X { get; set; } = double.NaN;
        public double Y { get; set; } = double.NaN;
        public double Z { get; set; } = double.NaN;
        public double AngleA { get; set; }
        public double AngleC { get; set; }

        public ToolLocationCnc With(Point3d? point, double? angleC, double? angleA)
        {
            return new ToolLocationCnc
            {
                Point = point ?? this.Point,
                AngleC = angleC ?? this.AngleC,
                AngleA = angleA ?? this.AngleA
            };
        }

        public bool IsDefined => !double.IsNaN(X);

        public IToolLocation Origin => new ToolLocationCnc { Point = Point3d.Origin };

        public Matrix3d GetTransformMatrixFrom(IToolLocation location)
        {
            var from = (ToolLocationCnc)location;
            var mat1 = Matrix3d.Displacement(from.Point.GetVectorTo(Point));
            var mat2 = Matrix3d.Rotation((from.AngleC - AngleC).ToRad(), Vector3d.ZAxis, Point);
            var mat3 = Matrix3d.Rotation((from.AngleA - AngleA).ToRad(),
                Vector3d.XAxis.RotateBy(-AngleC.ToRad(), Vector3d.ZAxis), Point);

            //var mat1 = Matrix3d.Displacement(Point.GetVectorTo(millPosition.Point));
            //var mat2 = Matrix3d.Rotation(Graph.ToRad(AngleC - millPosition.AngleC), Vector3d.ZAxis, millPosition.Point);
            //var mat3 = Matrix3d.Rotation(Graph.ToRad(millPosition.AngleA - AngleA), Vector3d.XAxis.RotateBy(Graph.ToRad(-millPosition.AngleC), Vector3d.ZAxis), millPosition.Point);

            return mat3 * mat2 * mat1;
        }
    }
}
