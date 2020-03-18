using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public struct Location
    {
        public Point3d Point { get; set; }
        public double AngleC { get; set; }
        public double AngleA { get; set; }

        public void Set(Point3d? point, double? angleC, double? angleA)
        {
            Point = point ?? Point;
            AngleC = angleC ?? AngleC;
            AngleA = angleA ?? AngleA;
        }

        public bool IsDefined => !double.IsNaN(Point.X) && !double.IsNaN(Point.Y) && !double.IsNaN(Point.Z);
    }
}