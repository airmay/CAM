﻿using Autodesk.AutoCAD.Geometry;
using CAM.Core;

namespace CAM.CncWorkCenter
{
    public class ToolLocationWireSaw
    {
        public Point2d OriginPoint { get; set; }
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

        //public Point3d Point => new Point3d(X, Y, Z);

        //public void Set(double u, double v, double angle)
        //{
        //    var point = new Point3d(Center.X - U, Center.Y, V);
        //    var angle = _angle.ToRad(); // Vector2d.YAxis.MinusPiToPiAngleTo(Vector);
        //    return new CableToolPosition(point, Center.ToPoint3d(), angle);
        //}

        public ToolLocationParams? GetParams()
        {
            return IsDefined ? new ToolLocationParams(U, V, Angle, 0, 0) : (ToolLocationParams?)null;
        }

        public bool IsDefined => !double.IsNaN(U);
    }
}
