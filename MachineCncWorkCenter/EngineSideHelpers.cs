using Autodesk.AutoCAD.DatabaseServices;
using System;
using Autodesk.AutoCAD.Geometry;

namespace CAM.Utils;

public static class EngineSideHelpers
{
    public static Side GetEngineSide(this double angle) => angle.ToRoundDeg() is > 0 and <= 180 ? Side.Right : Side.Left;

    public static double GetToolAngle(this Curve curve, Point3d point, Side? engineSide = null) => curve.GetTangent(point).Angle.GetToolAngle(engineSide);

    public static double GetToolAngle(this double angle, Side? engineSide = null)
    {
        engineSide ??= angle.GetEngineSide();
        return ((engineSide == Side.Right ? 1 : 2) * Math.PI + 2 * Math.PI - angle) % (2 * Math.PI);
    }

    public static Side GetEngineSide(this Curve curve)
    {
        return curve switch
        {
            Arc arc => CalcArc(arc),
            Line line => line.Angle.GetEngineSide(),
            Polyline polyline => CalcPolyline(polyline),
            _ => throw new InvalidOperationException($"Кривая типа {curve.GetType()} не может быть обработана.")
        };
    }

    private static Side CalcArc(Arc arc)
    {
        var startSide = arc.StartAngle.CosSign();
        var endSide = arc.EndAngle.CosSign();
        var cornersOneSide = Math.Sign(startSide * endSide);

        if (arc.TotalAngle.ToRoundDeg() > 180 && cornersOneSide > 0)
            throw new InvalidOperationException("Обработка дуги невозможна - дуга пересекает углы 90 и 270 градусов.");

        if (cornersOneSide < 0) //  дуга пересекает углы 90 или 270 градусов
            return startSide > 0 ? Side.Left : Side.Right;

        if (startSide == 0 && endSide == 0)
            return arc.StartAngle < Math.PI ? Side.Left : Side.Right;

        return startSide + endSide > 0 ? Side.Right : Side.Left;
    }

    private static Side CalcPolyline(Polyline polyline)
    {
        var sign = 0;
        Side? engineSide = null;

        for (var i = 0; i < polyline.NumberOfVertices; i++)
        {
            var point = polyline.GetPoint3dAt(i);
            var s = Math.Sign(Math.Sin(polyline.GetTangent(point).Angle.Round(3)));
            if (s == 0)
                continue;
            if (sign == 0)
            {
                sign = s;
                continue;
            }

            var bulge = polyline.GetBulgeAt(i - 1);
            if (bulge == 0)
                bulge = polyline.GetBulgeAt(i);

            if (s != sign)
            {
                var sd = sign > 0 ^ bulge < 0 ? Side.Left : Side.Right;
                if (engineSide != null)
                {
                    if (engineSide != sd)
                        throw new InvalidOperationException("Обработка полилинии невозможна.");
                }
                else
                    engineSide = sd;

                sign = s;
            }
            else if (Math.Abs(bulge) > 1)
                throw new InvalidOperationException("Обработка невозможна - дуга полилинии пересекает углы 90 и 270 градусов.");
        }

        return engineSide ?? polyline.GetTangent(polyline.StartPoint).Angle.GetEngineSide();
    }
}