using Autodesk.AutoCAD.Geometry;
using CAM.Core;

namespace CAM.CncWorkCenter
{
    public class ToolLocationCnc
    {
        public double X { get; set; } = double.NaN;
        public double Y { get; set; } = double.NaN;
        public double Z { get; set; } = double.NaN;
        public double AngleA { get; set; }
        public double AngleC { get; set; }

        public ToolLocationCnc() { }
        public ToolLocationCnc(ToolPosition? locationParams)
        {
            X = locationParams?.Point.X ?? 0;
            Y = locationParams?.Point.Y ?? 0;
            Z = locationParams?.Point.Z ?? 0;
            /*AngleC = locationParams?.Angle ?? 0;
            AngleA = locationParams?.Angle2 ?? 0;*/
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

        public ToolPosition? GetParams()
        {
            return IsDefined ? new ToolPosition(Point, AngleC, AngleA) : (ToolPosition?)null;
        }

        public bool IsDefined => !double.IsNaN(X);
    }
}
