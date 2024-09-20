using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;

namespace CAM.CncWorkCenter
{
    public class ToolLocationCnc
    {
        public Point3d Point { get; set; } = Algorithms.NullPoint3d;
        public double AngleA { get; set; }
        public double AngleC { get; set; }
    }
}
