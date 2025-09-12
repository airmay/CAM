using Autodesk.AutoCAD.Geometry;
using System;

namespace CAM.Core
{
    [Serializable]
    public struct ToolPosition
    {
        public ToolPosition(Point3d point, double angleC, double angleA = 0)
        {
            Point = point;
            AngleC = angleC;
            AngleA = angleA;
        }

        public Point3d Point { get; }
        public double AngleC { get; }
        public double AngleA { get; }
    }
}
