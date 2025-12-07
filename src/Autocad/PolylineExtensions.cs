using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Autocad.AutoCADCommands;
using CAM.Core.Processing;

namespace CAM.Autocad;

public static class PolylineExtensions
{
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

    public static ObjectId? CreateHatch(Polyline polyline, int side)
    {
        const int hatchSize = 40;
        try
        {
            ProcessingObjectBuilder.AddEntity(polyline);
            var offsetPolyline = polyline.GetOffsetCurves(hatchSize * side)[0] as Polyline;
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
                ProcessingObjectBuilder.AddEntity(offsetPolyline);

            var hatch = new Hatch();
            hatch.SetDatabaseDefaults();
            hatch.Normal = new Vector3d(0, 0, 1);
            hatch.Elevation = 0.0;
            hatch.Associative = false;
            hatch.PatternScale = 4;
            hatch.SetHatchPattern(HatchPatternType.PreDefined, "ANSI31");
            //hatch.PatternAngle = angle; // PatternAngle has to be after SetHatchPattern(). This is AutoCAD .NET SDK violating Framework Design Guidelines, which requires properties to be set in arbitrary order.
            hatch.Transparency = new Transparency(255 * (100 - 70) / 100);
            hatch.HatchStyle = HatchStyle.Outer;
            hatch.AppendLoop(HatchLoopTypes.External, new ObjectIdCollection(new[] { polyline.ObjectId }));
            if (offsetPolyline != null)
                hatch.AppendLoop(HatchLoopTypes.External, new ObjectIdCollection(new[] { offsetPolyline.ObjectId }));
            hatch.EvaluateHatch(true);
            ProcessingObjectBuilder.AddEntity(hatch);

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
}