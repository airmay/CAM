using Autodesk.AutoCAD.Geometry;

namespace CAM.Utils
{
    public class Pass
    {
        public Point3d Start { get; set; }
        public Point3d End { get; set; }
        public Pass(Point3d start, Point3d end)
        {
            Start = start;
            End = end;
        }
    }
}
