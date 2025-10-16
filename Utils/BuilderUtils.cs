using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM
{
    public static class BuilderUtils
    {
        public static IEnumerable<KeyValuePair<double, int>> GetPassList(IEnumerable<SawingMode> modes, double DepthAll, bool isZeroPass)
        {
            var passList = new List<KeyValuePair<double, int>>();
            var enumerator = modes.OrderBy(p => p.Depth ?? double.MaxValue).GetEnumerator();
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Не заданы режимы обработки");
            var mode = enumerator.Current;
            SawingMode nextMode = null;
            if (enumerator.MoveNext())
                nextMode = enumerator.Current;
            var depth = isZeroPass ? -mode.DepthStep : 0;
            do
            {
                depth += mode.DepthStep;
                if (nextMode != null && depth >= mode.Depth)
                {
                    mode = nextMode;
                    nextMode = enumerator.MoveNext() ? enumerator.Current : null;
                }
                if (depth > DepthAll)
                    depth = DepthAll;
                yield return new KeyValuePair<double, int>(depth, mode.Feed);
            }
            while (depth < DepthAll);
        }

        public static Side CalcEngineSide(double angle)
        {
            var deg = angle.ToRoundDeg();
            return deg > 0 && deg <= 180
                ? Side.Right
                : Side.Left;
        }

        public static double CalcToolAngle(Curve curve, Point3d point, Side engineSide = Side.None)
            => CalcToolAngle(curve.GetTangent(point).Angle, engineSide);

        public static double CalcToolAngle(double angle, Side engineSide = Side.None)
        {
            if (engineSide == Side.None)
                engineSide = CalcEngineSide(angle);
            return ((engineSide == Side.Right ? 1 : 2) * Math.PI + 2 * Math.PI - angle) % (2 * Math.PI);
        }

        public static List<Point2d> GetProcessPoints1(Curve profile, int index, double step, double shift, bool isMinToolCoord, double? begin, double? end, bool isProfileStep = false) //, bool isExactlyBegin, bool isExactlyEnd)
        {
            var result = new List<Point2d>();
            var start = 10000D;
            var rayVector = index == 0 ? -Vector2d.YAxis : -Vector2d.XAxis;

            var p0 = begin ?? Math.Max(profile.StartPoint[index], profile.EndPoint[index]);
            var p1 = end ?? Math.Min(profile.StartPoint[index], profile.EndPoint[index]);
            var dir = Math.Sign(p1 - p0);

            using (var curve = profile.ToCompositeCurve2d())
            using (var ray = new Ray2d())
            using (var intersector = new CurveCurveIntersector2d())
            {
                var pos = p0;
                Point2d? point0 = null;
                var length = profile.Length();
                int curveSign = dir * Math.Sign(curve.EndPoint[index] - curve.StartPoint[index]);
                double? dist = null;

                while (dir > 0 ? pos < p1 : pos > p1)
                {
                    double? max = null;
                    for (int i = 0; i <= 10; i++)
                    {
                        var rayPoint = GetPoint(pos + i * (shift / 10) * dir, start, index);
                        ray.Set(rayPoint, rayVector);
                        intersector.Set(curve, ray);
                        if (intersector.NumberOfIntersectionPoints > 0)
                        {
                            var intersect = intersector.GetIntersectionPoint(0);
                            max = Math.Max(max ?? double.MinValue, intersect[1 - index]);
                            if (!point0.HasValue && i == 0)
                                point0 = intersect;
                        }
                    }
                    if (max.HasValue)
                    {
                        var toolCoord = pos + shift * dir * (isMinToolCoord ^ dir < 0 ? 0 : 1);
                        result.Add(GetPoint(toolCoord, max.Value, index));
                    }
                    if (isProfileStep && point0.HasValue)
                    {
                        if (!dist.HasValue)
                            dist = curve.GetLength(0, curve.GetParameterOf(point0.Value));
                        dist += step * curveSign;
                        if (dist < 0 || dist > length)
                            break;
                        var point = curve.EvaluatePoint(curve.GetParameterAtLength(0, dist.Value, true));
                        pos = point[index];
                    }
                    else
                        pos += step * dir;
                }
            }
            return result;

        }

        private static Point2d GetPoint(double coord0, double coord1, int index)
        {
            var coord = new double[2];
            coord[index] = coord0;
            coord[1 - index] = coord1;
            return new Point2d(coord);
        }

        public static List<Point2d> GetProcessPoints(Curve profile, int index, double step, double shift, bool isMinToolCoord, double? begin, double? end, bool isExactlyBegin, bool isExactlyEnd, bool isProfileStep = false)
        {
            var result = new List<Point2d>();

            var p0 = begin ?? Math.Max(profile.StartPoint[index], profile.EndPoint[index]);
            var p1 = end ?? Math.Min(profile.StartPoint[index], profile.EndPoint[index]);
            var dir = Math.Sign(p1 - p0);
            p0 *= dir;
            p1 *= dir;
            var polyline = profile as Polyline;
            var points = polyline.GetPolylineFitPoints(1).Select(p => new Point2d(p[index] * dir, p[1 - index])).ToList();
            if (points.Last().X < points.First().X)
                points.Reverse();

            const double inf = 100000;
            points.Insert(0, new Point2d(-inf, points.First().Y));
            points.Add(new Point2d(inf, points.Last().Y));

            if (!isExactlyBegin)
                p0 -= shift;
            if (isExactlyEnd)
                p1 -= shift;

            var ind = 1;
            var pos = p0 - step;
            do
            {
                pos += step;
                if (pos > p1)
                    pos = p1;

                var max = Iterate(ref ind, pos);
                var ind2 = ind;
                max = Iterate(ref ind2, pos + shift, max);

                var toolCoord = isMinToolCoord ^ dir < 0 ? pos : pos + shift;
                result.Add(GetPoint(toolCoord * dir, max, index));
            }
            while (pos < p1 - 0.1);

            return result;         
            
            double Iterate(ref int i, double p, double? max = null)
            {
                while (points[i].X <= p)
                {
                    if (max.HasValue)
                        max = Math.Max(max.Value, points[i].Y);
                    i++;
                }
                var m = points[i - 1].Y + (points[i].Y - points[i - 1].Y) / (points[i].X - points[i - 1].X) * (p - points[i - 1].X);
                return max.HasValue ? Math.Max(max.Value, m) : m;
            }
        }
    }

    public class Interval
    {
        public Point2d Start { get; }

        public Point2d End { get; }

        private readonly double _koeff;

        public Interval(Point3d p1, Point3d p2, int index)
        {
            var start = p2[index] >= p1[index] ? p1 : p2;
            var end = p2[index] >= p1[index] ? p2 : p1;

            Start = new Point2d(start[index], start[1 - index]);
            End = new Point2d(end[index], end[1 - index]);

            if (End.X != Start.X)
                _koeff = (End.Y - Start.Y) / (End.X - Start.X);
        }

        public bool IsIn(double x)
        {
            return x >= Start.X && x < End.X;
        }

        public double GetY(double x)
        {
            return Start.Y + _koeff * (x - Start.X);
        }
    }
}