using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CAM.Utils;

public sealed class MyBinder : SerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName) => Type.GetType($"{typeName}, {assemblyName}");
}

public static class Helpers
{
    public static IEnumerable<Point2d> FindMax(this IEnumerable<double> xs, Point2d[] points, double width)
    {
        var index = 0;
        foreach(var x in xs)
            yield return new Point2d(x, FindMax(points, x, x + width, ref index));
    }

    public static double FindMax(Point2d[] points, double x1, double x2, ref int startIndex)
    {
        var i = startIndex;
        while (i < points.Length && (points[i].X < x1 || points[i].X.IsEqual(x1)))
            i++;

        if (i >= points.Length)
            return points[points.Length - 1].X.IsEqual(x1) ? points[points.Length - 1].Y : double.MinValue;

        var max = i == 0
            ? points[0].Y
            : LinearInterpolate(x1, points[i - 1], points[i]);
        if (i > 0)
            startIndex = i - 1;

        while (i < points.Length && points[i].X < x2 && points[i].X.IsNotEqual(x2))
            max = Math.Max(points[i++].Y, max);

        if (i < points.Length && i > 0)
            max = Math.Max(LinearInterpolate(x2, points[i - 1], points[i]), max);

        return max;
    }

    public static double LinearInterpolate(double x, Point2d point0, Point2d point1) => point0.Y + (point1.Y - point0.Y) * (x - point0.X) / (point1.X - point0.X);

    public static double LinearInterpolate(double x, double x0, double x1, double y0, double y1) => y0 + (y1 - y0) * (x - x0) / (x1 - x0);
}