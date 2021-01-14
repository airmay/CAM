using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Dreambuild.AutoCAD;

namespace CAM
{
    public static class BuilderUtils
    {
        public static IEnumerable<KeyValuePair<double, int>> GetPassList(IEnumerable<CuttingMode> modes, double DepthAll, bool isZeroPass)
        {
            var passList = new List<KeyValuePair<double, int>>();
            var enumerator = modes.OrderBy(p => p.Depth ?? double.MaxValue).GetEnumerator();
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Не заданы режимы обработки");
            var mode = enumerator.Current;
            CuttingMode nextMode = null;
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
            angle = angle.Round(6);
            var upDownSign = Math.Sign(Math.Sin(angle)); // переделать
            return upDownSign > 0 ? Side.Right : upDownSign < 0 ? Side.Left : Math.Cos(angle) > 0 ? Side.Left : Side.Right;
        }

        public static double CalcToolAngle(Curve curve, Point3d point, Side engineSide = Side.None) 
            => CalcToolAngle(curve.GetFirstDerivative(point).ToVector2d().Angle, engineSide);

        public static double CalcToolAngle(double angle, Side engineSide = Side.None)
        {
            if (engineSide == Side.None)
                engineSide = CalcEngineSide(angle);
            return ((engineSide == Side.Right ? 180 : 360) + 360 - angle.Round(6).ToDeg()) % 360;
        }

        public static List<Point2d> GetProcessPoints(Curve profile, int index, double step, double shift, bool isMinToolCoord, double? begin, double? end, bool isProfileStep = false) //, bool isExactlyBegin, bool isExactlyEnd)
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
                    for (int i = 0; i < 3; i++)
                    {
                        var rayPoint = GetPoint(pos + i * shift * dir, start);
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
                        var toolCoord = pos + shift * dir * (isMinToolCoord ^ dir < 0 ? 0 : 2);
                        result.Add(GetPoint(toolCoord, max.Value));
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

            Point2d GetPoint(double coord0, double coord1)
            {
                var coord = new double[2];
                coord[index] = coord0;
                coord[1 - index] = coord1;
                return new Point2d(coord);
            }
        }
    }
}
