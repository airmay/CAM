using Autodesk.AutoCAD.Geometry;
using CAM.Core;

namespace CAM.CncWorkCenter
{
    public class ToolLocationWireSaw
    {
        public double U { get; set; }
        public double V { get; set; }
        public double Angle { get; set; }

        public ToolLocationWireSaw() { }
        public ToolLocationWireSaw(ToolLocationParams? locationParams)
        {
            U = locationParams?.Param1 ?? 0;
            V = locationParams?.Param2 ?? 0;
            Angle = locationParams?.Param3 ?? 0;
        }

        public Point3d Point => new Point3d(X, Y, Z);

        //public void Set(Point3d? point, double? angleC, double? angleA)
        //{
        //    X = point?.X ?? X;
        //    Y = point?.Y ?? Y;
        //    Z = point?.Z ?? Z;
        //    AngleC = angleC ?? AngleC;
        //    AngleA = angleA ?? AngleA;
        //}

        public ToolLocationParams? GetParams()
        {
            return IsDefined ? new ToolLocationParams(U, V, Angle, 0, 0) : (ToolLocationParams?)null;
        }

        public bool IsDefined => !double.IsNaN(U);
    }
}
