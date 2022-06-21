using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;

namespace CAM
{
    public abstract class ToolPosition
    {
        public Point3d Point { get; set; }

        public Matrix3d Matrix { get; set; }

        public Matrix3d InvMatrix { get; set; }

        public bool IsDefined = true;

        //public Point3d Center { get; set; } = new Point3d(double.NaN, double.NaN, double.NaN);
        //public double AngleC { get; set; }
        //public double AngleA { get; set; }



        //public bool IsDefined => !double.IsNaN(Point.X) && !double.IsNaN(Point.Y) && !double.IsNaN(Point.Z);

        //public abstract Matrix3d GetTransformMatrix();

        public ToolPosition(Point3d point)
        {
            Point = point;
        }

        public ToolPosition Clone() => (ToolPosition)this.MemberwiseClone();
    }
}