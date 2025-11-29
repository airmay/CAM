using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal.PropertyInspector;
using CAM.Autocad.AutoCADCommands;
using CAM.Utils;

namespace CAM.Autocad;

public class Point3dComparer : IEqualityComparer<Point3d>
{
    public bool Equals(Point3d point1, Point3d point2) => point1.IsEqualTo(point2);

    public int GetHashCode(Point3d point)
    {
        var epsilon = Tolerance.Global.EqualVector;
        return (Math.Round(point.X / epsilon), Math.Round(point.X / epsilon), Math.Round(point.X / epsilon)).GetHashCode();
    }
}
public class Point2dComparer : IEqualityComparer<Point2d>
{
    public bool Equals(Point2d point1, Point2d point2) => point1.IsEqualTo(point2);

    public int GetHashCode(Point2d point)
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
    #region Angle

    public static double ToRad(this double angle) => angle * Math.PI / 180;
    public static double ToRad(this int angle) => ToRad((double)angle);
    public static double ToDeg(this double angle) => angle * 180 / Math.PI;
    public static double ToDeg(this double angle, int digits) => angle.ToDeg().Round(digits) % 360;
    public static double ToRoundDeg(this double angle) => angle.ToDeg(3);

    public static int CosSign(this double angle)
    {
        return angle.ToRoundDeg() switch
        {
            < 90 or > 270 => 1,
            > 90 and < 270 => -1,
            _ => 0
        };
    }

    #endregion

    #region Point3d

    public static readonly IEqualityComparer<Point3d> Point3dComparer = new Point3dComparer();
    public static readonly IEqualityComparer<Point2d> Point2dComparer = new Point2dComparer();

    public static Point3d WithZ(this Point3d point, double z) => new(point.X, point.Y, z);
    public static Point3d WithZ(this Point2d point, double z) => new(point.X, point.Y, z);

    public static Point3d GetClosestPoint(this Point3d point, params Point3d[] points) => points.OrderBy(p => p.DistanceTo(point)).First();

    public static Point3d GetCenter(IEnumerable<Point3d> points)
    {
        return points.Aggregate(Point3d.Origin, (sum, point) => sum.Add(point.GetAsVector())) / points.Count();
    }

    /// <summary>
    /// Получить точку с указанной координатой Z, расположенную на прямой заданной точкой и вектором
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
    #endregion

    public static bool IsLine(this ObjectId id) => id.ObjectClass.DxfName == AcadObjectNames.Line;

    public static Vector2d GetVector2d(this Line line) => new(line.Delta.X, line.Delta.Y);

    #region Curve

    public static Point3d[] Points(this Curve curve) => [curve.StartPoint, curve.EndPoint];

    public static Point3d GetPoint(this Curve curve, bool isStart) => isStart ? curve.StartPoint : curve.EndPoint;
    public static Point3d GetPoint(this Curve curve, int tip) => tip == 0 ? curve.StartPoint : curve.EndPoint;
    public static int GetTip(this Curve curve, Point3d point) => curve.IsStartPoint(point) ? 0 : 1;

    public static bool HasPoint(this Curve curve, Point3d point) => point.IsEqualTo(curve.StartPoint) || point.IsEqualTo(curve.EndPoint);

    public static bool IsStartPoint(this Curve curve, Point3d point) => point.IsEqualTo(curve.StartPoint);

    public static int GetDirection(this Curve curve, Point3d point) => curve.IsStartPoint(point).GetSign();

    public static Point3d NextPoint(this Curve curve, Point3d point) =>
        point == curve.StartPoint ? curve.EndPoint :
        point == curve.EndPoint ? curve.StartPoint :
        throw new ArgumentException($"Ошибка NextPoint: Точка {point} не принадлежит кривой {curve}");

    public static Point3d GetClosestPoint(this Curve curve, Point3d point) =>
        point.DistanceTo(curve.StartPoint) <= point.DistanceTo(curve.EndPoint) ? curve.StartPoint : curve.EndPoint;

    public static double Length(this Curve curve) => curve.GetDistanceAtParameter(curve.EndParam) - curve.GetDistanceAtParameter(curve.StartParam);

    public static Vector2d GetTangent(this Curve curve, Point3d point) => curve.GetFirstDerivative(point).ToVector2d();

    public static IEnumerable<Point3d> GetPoints(this Curve cv, int divs)
    {
        if (cv is Spline spline)
            cv = spline.ToPolyline();

        var div = (cv.EndParam - cv.StartParam) / divs;
        for (var i = 0; i <= divs; i++)
        {
            yield return cv.GetPointAtParam(cv.StartParam + i * div);
        }
    }

    public static T CreateCopy<T>(this T curve, double offset = 0, double dz = 0) where T: Curve
    {
        var copy = (T)curve.GetOffsetCurves(offset * (curve is Line).GetSign())[0];
        if (dz != 0)
            copy.TransformBy(Matrix3d.Displacement(Vector3d.ZAxis * dz));

        return copy;
    }

    public static Point3d GetOffsetPoint(this Curve curve, Point3d point, double offset)
    {
        return point + curve.GetFirstDerivative(point).GetNormal().RotateBy(Math.PI / 2, Vector3d.ZAxis) * offset;
    }
    #endregion

    #region GetDesc

    public static string GetDesc(this ObjectId id)
    {
        if (id.IsErased)
            Acad.RecoveryObject(id);
        return ObjectPropertyManagerProperties.GetDisplayName(id);
    }

    public static string GetDesc(this IEnumerable<ObjectId> ids)
    {
        return string.Join(",",
            ids.GroupBy(p => p.GetDesc(), (k, c) => new { name = $"{k}({c.Count()})", count = c.Count() })
                .OrderByDescending(p => p.count)
                .Select(p => p.name));
    }

    #endregion

    #region GetExtended

    public static Point3d ExpandStart(this Line line, double value) => line.StartPoint.GetExtendedPoint(line.EndPoint, value);

    public static Point3d ExpandEnd(this Line line, double value) => line.EndPoint.GetExtendedPoint(line.StartPoint, value);

    public static Point3d GetExtendedPoint(this Point3d point, Point3d basePoint, double value) => point + (point - basePoint).GetNormal() * value;

    #endregion

    #region GetSide

    /// <summary>
    /// Рассчитывает с какой стороны находится точка относительно кривой
    /// </summary>
    /// <returns>1 если точка слева от кривой<br/>-1 если точка справа от кривой</returns>
    public static int GetSide(this Curve curve, Point3d point) => (curve.EndPoint - curve.StartPoint).GetSide(point - curve.StartPoint);

    /// <summary>
    /// Рассчитывает с какой стороны находится точка относительно вектора заданного начальной и конечно точками
    /// </summary>
    /// <returns>1 если точка слева от вектора<br/>-1 если точка справа от вектора</returns>
    public static int GetSide(this Point3d point, Point3d start, Point3d end) => (end - start).GetSide(point - start);

    /// <summary>
    /// Рассчитывает в какую сторону направлен полученный вектор относительно заданного
    /// </summary>
    /// <returns>1 если вектор направлен налево<br/>-1 если вектор направлен направо</returns>
    public static int GetSide(this Vector3d vector0, Vector3d vector) => Math.Sign(vector0.ToVector2d().Kross(vector.ToVector2d()));

    public static bool IsTurnRight(Point3d px, Point3d py, Point3d pz)
    {
        double num = 0;
        num = ((pz.Y - py.Y) * (py.X - px.X)) - ((py.Y - px.Y) * (pz.X - py.X));
        return (num < 0f);
    }

    public static bool IsTurnRight(this Line line, Point3d point) => IsTurnRight(line.StartPoint, line.EndPoint, point);

    public static bool IsTurnRight(this Vector2d vector1, Vector2d vector2) => vector1.Kross(vector2) < 0;

    #endregion

    public static string GetSize(this AcadObject processingArea)
    {
        if (processingArea == null)
            return "";
        var bounds = processingArea.ObjectIds.GetExtents();
        var vector = bounds.MaxPoint - bounds.MinPoint;
        return $"{vector.X.Round()} x {vector.Y.Round()} x {vector.Z.Round()}";
    }
}