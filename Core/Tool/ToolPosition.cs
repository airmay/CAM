using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;

namespace CAM
{
    public abstract class ToolPositionOld
    {
        public double? X { get; set; }

        public double? Y { get; set; }

        public double? Z { get; set; }

        public Point3d Point 
        {
            get => new Point3d(X.Value, Y.Value, Z.Value);
            //get => (X.HasValue && Y.HasValue && Z.HasValue) ? (Point3d?)new Point3d(X.Value, Y.Value, Z.Value) : null;

            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            } 
            //set
            //{
            //    X = value.Value.X;
            //    Y = value.Value.Y;
            //    Z = value.Value.Z;
            //} 
        }

        public Matrix3d Matrix { get; set; }

        public Matrix3d InvMatrix { get; set; }

        //public bool IsDefined = true;

        //public Point3d Center { get; set; } = new Point3d(double.NaN, double.NaN, double.NaN);
        //public double AngleC { get; set; }
        //public double AngleA { get; set; }

        public bool IsDefined => X.HasValue && Y.HasValue && Z.HasValue;

        public abstract Matrix3d GetTransformMatrixFrom(ToolPositionOld toolPosition);

        public ToolPositionOld() { }

        public ToolPositionOld(Point3d point)
        {
            Point = point;
        }

        public ToolPositionOld Clone() => (ToolPositionOld)this.MemberwiseClone();
    }
}