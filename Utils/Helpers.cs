using Autodesk.AutoCAD.Geometry;
using System;
using System.Runtime.Serialization;

namespace CAM.Utils;

public sealed class MyBinder : SerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName) => Type.GetType($"{typeName}, {assemblyName}");
}

public static class Helpers
{
    public static (double, int) FindMax(Point2d[] points, double x1, double x2, int startIndex = 0)
    {
        var i = startIndex;
        while (points[i].X < x1)
            i++;

        var (max, index) = i == 0
            ? (points[0].Y, 0)
            : (LinearInterpolate(x1, points[i - 1], points[i]), i - 1);

        while (i < points.Length && points[i].X <= x2)
            max = Math.Max(points[i++].Y, max);

        if (i < points.Length)
            max = Math.Max(LinearInterpolate(x2, points[i - 1], points[i]), max);

        return (max, index);
    }

    public static double LinearInterpolate(double x, Point2d point0, Point2d point1) => point0.Y + (point1.Y - point0.Y) * (x - point0.X) / (point1.X - point0.X);

    public static double LinearInterpolate(double x, double x0, double x1, double y0, double y1) => y0 + (y1 - y0) * (x - x0) / (x1 - x0);
}
