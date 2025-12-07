using System;
using Autodesk.AutoCAD.Geometry;

namespace CAM.Core.Tools;

[Serializable]
public readonly struct ToolPosition(Point3d point, double angleC, double angleA = 0)
{
    public double X { get; } = point.X;
    public double Y { get; } = point.Y;
    public double Z { get; } = point.Z;
    public double AngleC { get; } = angleC;
    public double AngleA { get; } = angleA;

    public Point3d Point => new(X, Y, Z);
}