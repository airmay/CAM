using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal.PropertyInspector;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM
{
    public class Point3dComparer : IEqualityComparer<Point3d>
    {
        public bool Equals(Point3d point1, Point3d point2) => point1.IsEqualTo(point2);

        public int GetHashCode(Point3d point)
        {
            var epsilon = Tolerance.Global.EqualVector;
            return (Math.Round(point.X / epsilon), Math.Round(point.X / epsilon), Math.Round(point.X / epsilon)).GetHashCode();
        }
    }

    /// <summary>
    /// Методы для работы с графикой
    /// </summary>
    public static class Graph
    {
        public static Point3d WithPoint2d(this Point3d point, Point2d point2d) => new Point3d(point2d.X, point2d.Y, point.Z);
        public static Point3d WithXY(this Point3d point, double x, double y) => new Point3d(x, y, point.Z);
        public static Point3d WithZ(this Point3d point, double z) => new Point3d(point.X, point.Y, z);
        public static Point3d WithZ(this Point2d point, double z) => new Point3d(point.X, point.Y, z);

        public static readonly IEqualityComparer<Point3d> Point3dComparer = new Point3dComparer();

        public static string GetDesc(this ObjectId id)
        {
            if (id.IsErased)
                Acad.RecoveryObject(id);
            return ObjectPropertyManagerProperties.GetDisplayName(id);
        }

        public static string GetDesc(this IEnumerable<ObjectId> ids) => string.Join(",",
            ids.GroupBy(p => p.GetDesc(), (k, c) => new { name = $"{k}({c.Count()})", count = c.Count() }).OrderByDescending(p => p.count).Select(p => p.name));

        public static bool IsLine(this ObjectId id) => id.ObjectClass.DxfName == AcadObjectNames.Line;

        public static Point3d GetClosestPoint(this Curve curve, Point3d point) => point.DistanceTo(curve.StartPoint) <= point.DistanceTo(curve.EndPoint) ? curve.StartPoint : curve.EndPoint;

        internal static double ToRad(this double angle) => angle * Math.PI / 180;

        internal static double ToRad(this int angle) => ToRad((double)angle);

        internal static double ToDeg(this double angle) => angle * 180 / Math.PI;

        internal static double ToDeg(this double angle, int digits) => angle.ToDeg().Round(digits) % 360;
        internal static double ToRoundDeg(this double angle) => angle.ToDeg(3);
        internal static int CosSign(this double angle)
        {
            var deg = angle.ToRoundDeg();
            return deg < 90 || deg > 270 
                ? 1
                : (deg > 90 && deg < 270) ? -1 : 0;
        }

        public static double Length(this Curve curve) => curve.GetDistanceAtParameter(curve.EndParam) - curve.GetDistanceAtParameter(curve.StartParam);

        public static Vector2d GetTangent(this Curve curve, Point3d point) => curve.GetFirstDerivative(point).ToVector2d();

        public static Vector2d GetTangent(this Curve curve, double param) => curve.GetFirstDerivative(param).ToVector2d();

        public static Vector2d GetTangent(this Curve curve, Corner corner) => curve.GetTangent(corner == Corner.Start ? curve.StartParam : curve.EndParam);

        public static IEnumerable<Point3d> GetStartEndPoints(this Curve curve)
        {
            yield return curve.StartPoint;
            yield return curve.EndPoint;
        }

        public static Point3d GetPoint(this Curve curve, CurveTip tip) => tip == CurveTip.Start ? curve.StartPoint : curve.EndPoint;

        public static CurveTip Swap(this CurveTip tip) => (CurveTip)(1 - (int)tip);

        public static Point3d GetPoint(this Curve curve, Corner corner) => corner == Corner.Start ? curve.StartPoint : curve.EndPoint;

        /// <summary>
        /// Получить точку с указанной координатой Z, расположенную на прямой заданной вектором
        /// </summary>
        /// <param name="p"></param>
        /// <param name="v"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Point3d GetPoint(this Point3d p, Vector3d v, double z)
        {
            var t = (z - p.Z) / v.Z;
            var x = p.X + v.X * t;
            var y = p.Y + v.Y * t;

            return new Point3d(x, y, z);
        }

        public static Corner GetCorner(this Curve curve, Point3d point) =>
            point == curve.StartPoint ? Corner.Start : (point == curve.EndPoint ? Corner.End : throw new ArgumentException($"Ошибка GetCorner: Точка {point} не принадлежит кривой {curve}"));

        public static bool HasPoint(this Curve curve, Point3d point) => point.IsEqualTo(curve.StartPoint) || point.IsEqualTo(curve.EndPoint);
        public static bool IsStartPoint(this Curve curve, Point3d point) => point.IsEqualTo(curve.StartPoint);

        public static Point3d NextPoint(this Curve curve, Point3d point) =>
            point == curve.StartPoint ? curve.EndPoint : (point == curve.EndPoint ? curve.StartPoint : throw new ArgumentException($"Ошибка NextPoint: Точка {point} не принадлежит кривой {curve}"));

        public static void SetPoint(this Curve curve, Corner corner, Point3d point)
        {
            if (corner == Corner.Start)
                curve.StartPoint = point;
            else
                curve.EndPoint = point;
        }

        public static Vector2d GetVector2d(this Line line) => new Vector2d(line.Delta.X, line.Delta.Y);

        public static double AngleRad(this Line line) => Math.Round(line.Angle, 6);

        public static double AngleDeg(this Line line) => Math.Round(line.Angle * 180 / Math.PI, 6);

        public static Point3d ExpandStart(this Line line, double value) => line.StartPoint.GetExtendedPoint(line.EndPoint, value);

        public static Point3d ExpandEnd(this Line line, double value) => line.EndPoint.GetExtendedPoint(line.StartPoint, value);

        public static Point3d GetExtendedPoint(this Point3d point, Point3d basePoint, double value) => point + (point - basePoint).GetNormal() * value;

        public static Point3d GetClosestPoint(this Point3d point, params Point3d[] points) => points.OrderBy(p => p.DistanceTo(point)).First();

        public static bool IsEqual(this double value1, double value2) => Math.Abs(value1 - value2) < Consts.Epsilon;
        public static bool IsEqual(this Point3d point1, Point3d point2) => point1.X.IsEqual(point2.X) && point1.Y.IsEqual(point2.Y) && point1.Z.IsEqual(point2.Z);

        public static double GetAngle(Point3d p1, Point3d p2, Point3d p3) => p1.GetVectorTo(p2).GetAngleTo(p2.GetVectorTo(p3));

        public static bool IsTurnRight(Point3d px, Point3d py, Point3d pz)
        {
            double num = 0;
            num = ((pz.Y - py.Y) * (py.X - px.X)) - ((py.Y - px.Y) * (pz.X - py.X));
            return (num < 0f);
        }

        public static bool IsTurnRight(this Line line, Point3d point) => IsTurnRight(line.StartPoint, line.EndPoint, point);

        public static bool IsTurnRight(this Vector2d vector1, Vector2d vector2) => vector1.Kross(vector2) < 0;

        public static List<Point3d> Intersect(this Curve entity, List<Curve> entitys, Intersect intersectType = default)
        {
            var result = new List<Point3d>();
            entitys.ForEach(p => result.AddRange(entity.Intersect(p, intersectType)));
            return result;
        }

        public static IEnumerable<Point3d> GetPoints(this Curve cv, int divs)
        {
            if (cv is Spline spline)
                cv = spline.ToPolyline();
            
            double div = (cv.EndParam - cv.StartParam) / divs;
            for (int i = 0; i <= divs; i++)
            {
                yield return cv.GetPointAtParam(cv.StartParam + i * div);
            }
        }

        public static IEnumerable<Point3d> GetPolylineFitPoints(this Polyline poly, double distDelta)
        {
            for (int i = 0; i < poly.EndParam - Consts.Epsilon; i++)
            {
                if (poly.GetBulgeAt(i) == 0)
                {
                    yield return poly.GetPointAtParameter(i);
                }
                else
                {
                    for (var dist = poly.GetDistAtParam(i); dist < poly.GetDistAtParam(i + 1); dist += distDelta)
                    {
                        yield return poly.GetPointAtDistX(dist);
                    }
                }
            }
            yield return poly.GetPointAtParameter(poly.EndParam);
        }

        /// <summary>
        /// Converts line to polyline.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>A polyline.</returns>
        public static Polyline3d ToPolyline3d(this Line line)
        {
            var poly = new Polyline3d();
            poly.AppendVertex(new PolylineVertex3d(line.StartPoint));
            poly.AppendVertex(new PolylineVertex3d(line.EndPoint));
            return poly;
        }

        public static Polyline ToPolyline(this List<Curve> contour)
        {
            var startPoint = contour.Count > 1
                ? contour[1].HasPoint(contour[0].EndPoint) ? contour[0].StartPoint : contour[0].EndPoint
                : contour[0].StartPoint;

            var polyline = new Polyline();
            var point = startPoint;
            var index = 0;
            foreach (var curve in contour)
            {
                if (curve is Polyline pcurve)
                {
                    polyline.JoinPolyline(pcurve);
                    index = polyline.NumberOfVertices - 1;
                    polyline.RemoveVertexAt(index);
                }
                else
                {
                    var bulge = curve is Arc arc ? arc.GetArcBulge(point) : 0;
                    polyline.AddVertexAt(index++, point.ToPoint2d(), bulge, 0, 0);
                }

                point = curve.NextPoint(point);
            }
            if (point != startPoint)
                polyline.AddVertexAt(index, point.ToPoint2d(), 0, 0, 0);
            else
                polyline.Closed = point == startPoint;

            return polyline;
        }

        public static ObjectId? CreateHatch(Polyline polyline, Side side, Func<Entity, ObjectId> addEntity)
        {
            const int hatchSize = 40;
            try
            {
                addEntity(polyline);
                var offsetPolyline = polyline.GetOffsetCurves(-hatchSize * (int)side)[0] as Polyline;
                if (!polyline.Closed)
                {
                    offsetPolyline.ReverseCurve();
                    polyline.JoinPolyline(offsetPolyline);
                    polyline.SetBulgeAt(polyline.NumberOfVertices - 1, 0);
                    polyline.Closed = true;
                    offsetPolyline.Dispose();
                    offsetPolyline = null;
                }
                else
                    addEntity(offsetPolyline);

                var hatch = new Hatch();
                hatch.SetDatabaseDefaults();
                hatch.Normal = new Vector3d(0, 0, 1);
                hatch.Elevation = 0.0;
                hatch.Associative = false;
                hatch.PatternScale = 4;
                hatch.SetHatchPattern(HatchPatternType.PreDefined, "ANSI31");
                //hatch.PatternAngle = angle; // PatternAngle has to be after SetHatchPattern(). This is AutoCAD .NET SDK violating Framework Design Guidelines, which requires properties to be set in arbitrary order.
                hatch.HatchStyle = HatchStyle.Outer;
                hatch.AppendLoop(HatchLoopTypes.External, new ObjectIdCollection(new[] { polyline.ObjectId }));
                if (offsetPolyline != null)
                    hatch.AppendLoop(HatchLoopTypes.External, new ObjectIdCollection(new[] { offsetPolyline.ObjectId }));
                hatch.EvaluateHatch(true);
                addEntity(hatch);

                polyline.Erase();
                offsetPolyline?.Erase();

                return hatch.Id;
            }
            catch (Exception ex)
            {
                Acad.Alert("Ошибка при построении штриховки", ex);
                return null;
            }
        }

        #region Extensions to convert Curve to CompositeCurve2d

        public static bool IsBetween(this Point2d pt, Point2d p1, Point2d p2)
        {
            return ((pt - p1).GetNormal() == (p2 - pt).GetNormal());
        }
        public static Point2d To2d(this Point3d pt)
        {
            return (new Point2d(pt.X, pt.Y));
        }
        public static Point2d GetPointAtRelativeParam(this Curve c, double rpm)
        {
            double spm = c.StartParam;
            double epm = c.EndParam;
            return c.GetPointAtParameter(spm + (epm - spm) * rpm).To2d();
        }
        public static Curve2d[] ToCurve2dArray(this Arc a)
        {
            Point2d sp = a.GetPointAtRelativeParam(0.0);
            Point2d mp = a.GetPointAtRelativeParam(0.5);
            Point2d ep = a.GetPointAtRelativeParam(1.0);
            return new Curve2d[2]{
                new CircularArc2d(sp, mp, ep),
                new LineSegment2d(ep, sp)
            };
        }
        public static Curve2d[] ToCurve2dArray(this Circle ci)
        {
            Point2d sp = ci.GetPointAtRelativeParam(0.0);
            Point2d q1 = ci.GetPointAtRelativeParam(0.25);
            Point2d ep = ci.GetPointAtRelativeParam(0.5);
            Point2d q3 = ci.GetPointAtRelativeParam(0.75);
            return new Curve2d[2]{
                new CircularArc2d(sp, q1, ep),
                new CircularArc2d(ep, q3, sp)
            };
        }
        public static Curve2d[] ToCurve2dArray(this Curve c)
        {
            if (c.StartParam != 0.0 || (c.EndParam - Math.Truncate(c.EndParam) != 0.0))
                throw (new ArgumentException("Invalid Curve Parameter"));

            int n = (int)c.EndParam;
            Curve2d[] ca = new Curve2d[n];
            Point2d sp0 = (c.GetPointAtParameter(0.0)).To2d();
            Point2d sp = sp0;
            for (int i = 0; i < n; i++)
            {
                Point2d mp = (c.GetPointAtParameter(i + 0.5)).To2d();
                Point2d ep = (c.GetPointAtParameter(i + 1.0)).To2d();
                if (mp.IsBetween(sp, ep))
                    ca[i] = new LineSegment2d(sp, ep);
                else
                    ca[i] = new CircularArc2d(sp, mp, ep);
                sp = ep;
            }
            return ca;
        }
        public static CompositeCurve2d ToCompositeCurve2d(this Curve c)
        {
            Curve2d[] ca;
            Arc a = c as Arc;
            if (a != null)
                ca = a.ToCurve2dArray();
            else
            {
                Circle ci = c as Circle;
                if (ci != null)
                    ca = ci.ToCurve2dArray();
                else if (c is Line)
                    ca = new Curve2d[] { new LineSegment2d(c.StartPoint.To2d(), c.EndPoint.To2d()) };
                else
                    ca = c.ToCurve2dArray();
            }
            return new CompositeCurve2d(ca);
        }
        public static bool IsPointInside(this Curve c, Point2d pt)
        {
            int n = 0;
            using (CompositeCurve2d c2d = c.ToCompositeCurve2d())
            using (Ray2d r2d = new Ray2d(pt, Vector2d.XAxis))
            using (CurveCurveIntersector2d cci2d = new CurveCurveIntersector2d(c2d, r2d))
            {
                for (int i = 0; i < cci2d.NumberOfIntersectionPoints; i++)
                    if (cci2d.IsTransversal(i)) n++;
            }
            return (n % 2 != 0);
        }
        public static Point2d[] IntersectWith(this Curve2d curve, Curve2d intersectCurve)
        {
            using (CurveCurveIntersector2d intersector = new CurveCurveIntersector2d(curve, intersectCurve))
            {
                return Enumerable.Range(0, intersector.NumberOfIntersectionPoints).Select(i => intersector.GetIntersectionPoint(i)).ToArray();
            }
        }
        #endregion

        public static Point3d GetCenter(IEnumerable<Point3d> points)
        {
            return points.Aggregate(Point3d.Origin, (sum, point) => sum.Add(point.GetAsVector())) / points.Count();
        }
    }

    public enum CurveTip
    {
        Start,
        End
    }
}