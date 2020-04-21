using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public class Location
    {
        public Point3d Point { get; set; } = new Point3d(double.NaN, double.NaN, double.NaN);
        public double AngleC { get; set; }
        public double AngleA { get; set; }

        public void Set(Point3d? point, double? angleC, double? angleA)
        {
            Point = point ?? Point;
            AngleC = angleC ?? AngleC;
            AngleA = angleA ?? AngleA;
        }

        public bool IsDefined => !double.IsNaN(Point.X) && !double.IsNaN(Point.Y) && !double.IsNaN(Point.Z);

        public Location Clone() => (Location)this.MemberwiseClone();
    }
}