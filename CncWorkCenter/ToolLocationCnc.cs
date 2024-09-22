using Autodesk.AutoCAD.Geometry;

namespace CAM.CncWorkCenter
{
    public class ToolLocationCnc
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

        public bool IsDefined() => !double.IsNaN(X);

        public ToolLocationCnc Clone(Point3d? point, double? angleC, double? angleA)
        {
            return new ToolLocationCnc
            {
                Point = point ?? this.Point,
                AngleC = angleC ?? this.AngleC,
                AngleA = angleA ?? this.AngleA
            };
        }
    }
}
