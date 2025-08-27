using Autodesk.AutoCAD.Geometry;

namespace CAM.Core
{
    public struct ToolPosition
    {
        public ToolPosition(Point3d point, double angle, double angle2 = 0)
        {
            Point = point;
            Angle = angle;
            Angle2 = angle2;
        }

        public Point3d Point { get; }
        public double Angle { get; }
        public double Angle2 { get; }
    }
}
