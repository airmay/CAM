using Autodesk.AutoCAD.Geometry;
using CAM.Core;

namespace CAM.CncWorkCenter
{
    public class ToolLocationWireSaw
    {
        public double X { get; set; } = double.NaN;
        public double Y { get; set; } = double.NaN;
        public double Z { get; set; } = double.NaN;
        public double AngleA { get; set; }
        public double AngleC { get; set; }

        public ToolLocationWireSaw() { }
        public ToolLocationWireSaw(ToolLocationParams? locationParams)
        {
            X = locationParams?.Param1 ?? 0;
            Y = locationParams?.Param2 ?? 0;
            Z = locationParams?.Param3 ?? 0;
            AngleC = locationParams?.Param4 ?? 0;
            AngleA = locationParams?.Param5 ?? 0;
        }

        public Point3d Point => new Point3d(X, Y, Z);

        public void Set(Point3d? point, double? angleC, double? angleA)
        {
            X = point?.X ?? X;
            Y = point?.Y ?? Y;
            Z = point?.Z ?? Z;
            AngleC = angleC ?? AngleC;
            AngleA = angleA ?? AngleA;
        }

        public ToolLocationParams? GetParams()
        {
            return IsDefined ? new ToolLocationParams(X, Y, Z, AngleC, AngleA) : (ToolLocationParams?)null;
        }

        public bool IsDefined => !double.IsNaN(X);
    }
}
