using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Autocad.AutoCADCommands;
using DbSurface = Autodesk.AutoCAD.DatabaseServices.Surface;

namespace CAM.Autocad
{
    public static class AcadUtils
    {
        public static Matrix3d GetRotationMatrix(double angleC, double angleA) =>
            Matrix3d.Rotation(-angleA.ToRad(), Vector3d.XAxis, Point3d.Origin)
            * Matrix3d.Rotation(angleC.ToRad(), Vector3d.ZAxis, Point3d.Origin);

        public static DbSurface CreateOffsetSurface(this IEnumerable<ObjectId> objectIds, double offsetDistance)
        {
            DbSurface unionSurface = null;
            foreach (var dBObject in objectIds.QOpenForRead())
            {
                DbSurface surface;
                switch (dBObject)
                {
                    case DbSurface sf:
                        surface = sf.Clone() as DbSurface;
                        break;
                    case Region region:
                        surface = new PlaneSurface();
                        ((PlaneSurface)surface).CreateFromRegion(region);
                        break;
                    default:
                        throw new Exception($"Объект типа {dBObject.GetType()} не может быть обработан (1)");
                }
                if (unionSurface == null)
                    unionSurface = surface;
                else
                {
                    var res = unionSurface.BooleanUnion(surface);
                    if (res != null)
                    {
                        unionSurface.Dispose();
                        unionSurface = res;
                    }
                    surface.Dispose();
                }
            }
            if (offsetDistance == 0)
                return unionSurface;

            try
            {
                var offsetSurface = DbSurface.CreateOffsetSurface(unionSurface, offsetDistance) as DbSurface;
                unionSurface.Dispose();
                return offsetSurface;
            }
            catch
            {
                unionSurface.TransformBy(Matrix3d.Displacement(Vector3d.ZAxis * offsetDistance));
                return unionSurface;
            }
        }

        public static double? CalcMaxZ(this DbSurface surface, double x, params double[] yParams)
        {
            const double zMin = -10000D;
            return yParams.Select(y =>
                {
                    surface.RayTest(new Point3d(x, y, zMin), Vector3d.ZAxis, Consts.Epsilon,
                        out var col, out var par);
                    return par.Count > 0 ? (double?)par[par.Count - 1] + zMin : null;
                })
                .Max();
        }

        public static IEnumerable<Point3d> CalcOffsetPoints(this Point3dCollection points, double distance, double step)
        {
            if (points.Count < 3)
                return points.Cast<Point3d>().ToList();

            var polyline = new PolylineCurve3d(points);
            var offsetCurves = polyline.GetTrimmedOffset(distance, -Vector3d.YAxis, OffsetCurveExtensionType.Fillet);
            if (offsetCurves.Length == 0)
                return new List<Point3d> { points[0], points[points.Count - 1] };

            var offsetCurve = offsetCurves[0];
            var displacement = Matrix3d.Displacement(Vector3d.ZAxis.Negate() * distance);
            offsetCurve.TransformBy(displacement);

            return offsetCurve.GetPoints(step);
        }

        public static List<Point3d> Trim(this List<Point3d> points, double beginDist, double endDist)
        {
            var startSkip = 0;
            if (beginDist > 0)
            {
                var length = 0D;
                for (var i = 1; i < points.Count; i++)
                {
                    length += points[i].DistanceTo(points[i - 1]);
                    if (length >= beginDist)
                    {
                        startSkip = i;
                        break;
                    }
                }
            }

            var endSkip = 0;
            if (endDist > 0)
            {
                var length = 0D;
                for (var i = points.Count - 2; i > 1; i--)
                {
                    length += points[i].DistanceTo(points[i + 1]);
                    if (length >= endDist)
                    {
                        endSkip = points.Count - i - 1;
                        break;
                    }
                } 
            }

            return points.Skip(startSkip).Take(points.Count - startSkip - endSkip).ToList();
        }

        public static IEnumerable<Point3d> GetPoints(this Curve3d curve, double step)
        {
            return new List<Point3d> { curve.StartPoint }.Concat(GetPointsInner(curve));

            IEnumerable<Point3d> GetPointsInner(Curve3d c)
            {
                switch (c)
                {
                    case LineSegment3d line:
                        return new[] { line.EndPoint };
                    case CircularArc3d arc:
                        var startParam = arc.GetParameterOf(arc.StartPoint);
                        var endParam = arc.GetParameterOf(arc.EndPoint);
                        var length = arc.GetLength(startParam, endParam, Consts.Epsilon);
                        var number = (int)(length / (step * 2));
                        return number > 2
                            ? arc.GetSamplePoints(number).Skip(1).Select(p => p.Point)
                            : new[] { arc.EndPoint };
                    case CompositeCurve3d compositeCurve3d:
                        return compositeCurve3d.GetCurves().SelectMany(GetPointsInner);
                    default:
                        throw new Exception($"Полученный тип кривой не может быть обработан {c.GetType()}");
                }
            }
        }
    }
}
