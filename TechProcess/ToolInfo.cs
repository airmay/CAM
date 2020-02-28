using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public struct ToolInfo
    {
        public int Index { get; set; }

        public Point3d Point { get; set; }
        public double AngleC { get; set; }
        public double AngleA { get; set; }

        public void Set(Point3d? point, double? angleC, double? angleA)
        {
            Point = point ?? Point;
            AngleC = angleC ?? AngleC;
            AngleA = angleA ?? AngleA;
        }

        //public ToolPosition Clone() => (ToolPosition)this.MemberwiseClone();
    }
}