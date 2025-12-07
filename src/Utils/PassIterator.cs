using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.Geometry;

namespace CAM.Utils;

public class Pass(Point3d start, Point3d end)
{
    public Point3d Start { get; } = start;
    public Point3d End { get; } = end;
}

public static class PassIterator
{
    public static IEnumerable<Pass> Create(Point3d point1, Point3d point2)
    {
        while (true)
        {
            yield return new Pass(point1, point2);
            yield return new Pass(point2, point1);
        }
    }

    public static IEnumerable<Pass> WithOffset(this IEnumerable<Pass> source, Vector3d vector)
    {
        return source.Select((p, i) => new Pass(p.Start.Add(vector * i), p.End.Add(vector * i)));
    }

    public static IEnumerable<Pass> Take(this IEnumerable<Pass> source, Vector3d vector, Vector3d limit)
    {
        var count = (int)Math.Ceiling(limit.Length / vector.Length) + 1;
        return source.Select((p, i) =>
            {
                var offset = i < count - 1 ? vector * i : limit;
                return new Pass(p.Start.Add(offset), p.End.Add(offset));
            })
            .Take(count);
    }

    public static IEnumerable<Pass> TakeZ(this IEnumerable<Pass> source, double dz, double zMax)
    {
        return source.Take(-Vector3d.ZAxis * dz, -Vector3d.ZAxis * zMax);
    }

}