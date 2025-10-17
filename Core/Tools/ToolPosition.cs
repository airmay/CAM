using System;
using Autodesk.AutoCAD.Geometry;

namespace CAM.Core.Tools;

[Serializable]
public struct ToolPosition
{
    public ToolPosition(Point3d point, double angleC, double angleA = 0)
    {
        X = point.X;
        Y = point.Y;
        Z = point.Z;
        AngleC = angleC;
        AngleA = angleA;
    }

    public double X { get; }
    public double Y { get; }
    public double Z { get; }
    public Point3d Point => new Point3d(X, Y, Z);
    public double AngleC { get; }
    public double AngleA { get; }
}