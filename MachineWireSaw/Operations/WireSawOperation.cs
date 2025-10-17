using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CAM.Autocad;
using CAM.Autocad.AutoCADCommands;
using CAM.UI;
using CAM.Utils;
using DbSurface = Autodesk.AutoCAD.DatabaseServices.Surface;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace CAM.MachineWireSaw.Operations;

[Serializable]
public class WireSawOperation : OperationWireSawBase
{
    public bool IsReverseDirection { get; set; }
    public bool Across { get; set; }
    public bool IsReverseOffset { get; set; }
    public double? Angle { get; set; }
    public int StepCount { get; set; } = 100;
    public bool IsReverseU { get; set; }
    public bool IsReverseAngle { get; set; }
    //public double DU { get; set; }
    //public double Delay { get; set; } = 60;
    //public bool IsExtraMove { get; set; }
    //public bool IsExtraRotate { get; set; }

    public double Offset => TechProcess.Offset * IsReverseOffset.GetSign(-1);

    public static void ConfigureParamsView(ParamsControl view)
    {
        view.AddAcadObject();
        view.AddCheckBox(nameof(IsReverseDirection), "Обратное направление");
        view.AddCheckBox(nameof(Across), "Поперек");
        view.AddCheckBox(nameof(IsReverseOffset), "Обратный Offset");
        view.AddTextBox(nameof(Angle), "Угол");
        view.AddTextBox(nameof(StepCount), "Количество шагов");
        view.AddIndent();
        view.AddCheckBox(nameof(IsReverseU), "Обратное U");
        view.AddCheckBox(nameof(IsReverseAngle), "Обратный угол");
        //view.AddIndent();
        //view.AddTextBox(nameof(Delay), "Задержка");
        //view.AddCheckBox(nameof(IsExtraMove), "Возврат");
        //view.AddCheckBox(nameof(IsExtraRotate), "Поворот");
        //view.AddTextBox(nameof(DU), "dU");
    }

    public override void Execute()
    {
        var entities = ProcessingArea.ObjectIds.QOpenForRead<Entity>();
        var extents = entities.GetExtents();

        Processor.StartOperation(extents.MaxPoint.Z);

        //var surface = dBObject as DbSurface;
        //var plane = surface.GetPlane();
        //var n = plane.Normal;
        //if (surf is Plane)
        //{
        //    Plane plane = (Plane)dBObject;
        //    Vector3d normal = plane.Normal;
        //}

        // плоскость по области
        if (entities[0] is Region region)
        {
            if (region.Normal.IsParallelTo(Vector3d.ZAxis))
            {
                HorizontalPlaneCutting();
                return;
            }

            Point3d point;
            using (var exploded = new DBObjectCollection())
            {
                region.Explode(exploded);
                point = ((Curve)exploded[0]).StartPoint;
            }

            PlaneCutting(extents, region.Normal, point);

            return;
        }

        // плоскость по 2 отрезкам
        if (entities.Length > 1 && entities[0] is Line linе1 && entities[1] is Line linе2)
        {
            var normal = linе1.Delta.CrossProduct(linе2.StartPoint - linе1.StartPoint);
            // Если нормаль нулевая (нормаль нулевая если отрезки коллинеарны или один из них - точка) 
            if (normal.IsZeroLength())
                normal = linе1.Delta.CrossProduct(linе2.EndPoint - linе1.StartPoint);

            if (!normal.IsZeroLength())
            {
                // Смешанное произведение
                var tripleProduct = linе2.Delta.DotProduct(normal);
                // Если смешанное произведение близко к нулю - отрезки компланарны
                if (tripleProduct.IsZero())
                {
                    if (normal.IsParallelTo(Vector3d.ZAxis))
                        HorizontalPlaneCutting();
                    else
                        PlaneCutting(extents, normal, linе1.StartPoint);

                    return;
                }
            }
        }

        // плоскость по 1 отрезку
        if (entities.Length == 1 && entities[0] is Line linе)
        {
            if (linе.Delta.IsPerpendicularTo(Vector3d.ZAxis))
            {
                HorizontalPlaneCutting();
                return;
            }

            var toolDirection = Angle.HasValue
                ? Vector3d.YAxis.RotateBy(Angle.Value.ToRad(), Vector3d.ZAxis)
                : linе.Delta.GetPerpendicularVector();
            var normal = linе.Delta.CrossProduct(toolDirection).GetNormal();

            PlaneCutting(extents, normal, linе.StartPoint);
            return;
        }

        // поверхность
        if (entities[0] is DbSurface surface)
        {
            var exploded = new DBObjectCollection();
            surface.Explode(exploded);
            var curves = exploded.Cast<Curve>()
                .OrderByDescending(p => Math.Abs(p.EndPoint.Z - p.StartPoint.Z))
                .Take(2)
                .ToList();
            SurfaceCutting(curves);

            return;
        }

        // поверхность по 2 направляющим
        if (entities.Length > 1 && entities[0] is Curve curve0 && entities[1] is Curve curve1)
        {
            SurfaceCutting(new List<Curve> {curve0, curve1});
            return;
        }

        // поверхность по 1 направляющей
        if (entities[0] is Curve curve)
        {
            var toolDirection = Angle.HasValue
                ? Vector3d.YAxis.RotateBy(Angle.Value.ToRad(), Vector3d.ZAxis)
                : curve.GetFirstDerivative(curve.StartParam).GetNormal().GetPerpendicularVector();
            var matrix = Matrix3d.Displacement(toolDirection);
            var copy = curve.GetTransformedCopy(matrix) as Curve;
            SurfaceCutting(new List<Curve> { curve, copy});
            return;
        }

        throw new Exception(ErrorStatus.NotImplementedYet, "Нет реализации обработки для данной области");
    }

    private void HorizontalPlaneCutting()
    {
        var angle = Angle.GetValueOrDefault().ToRad();
        if (Across)
            angle += Math.PI / 2;
        if (IsReverseDirection)
            angle += Math.PI;

        var extents = new Extents3d();
        var matrix = Matrix3d.Rotation(-angle, Vector3d.ZAxis, TechProcess.Origin.Point.ToPoint3d());
        ProcessingArea.ObjectIds.ForEach<Entity>(p => extents.AddExtents(p.GetTransformedCopy(matrix).GeometricExtents));
        matrix = matrix.Inverse();
        var startPoint = extents.MaxPoint.TransformBy(matrix) + Offset * Vector3d.ZAxis;
        var endPoint = extents.MinPoint.TransformBy(matrix) + Offset * Vector3d.ZAxis;
        var direction = -Vector3d.XAxis.RotateBy(angle, Vector3d.ZAxis);
        var toolDirection = direction.GetPerpendicularVector();

        Processor.Move(startPoint - TechProcess.Approach * direction, toolDirection, IsReverseAngle, IsReverseU);
        Processor.Cutting(startPoint, toolDirection);
        Processor.Cutting(endPoint, toolDirection);
        Processor.Cutting(endPoint + TechProcess.Departure * direction, toolDirection);
    }

    private void PlaneCutting(Extents3d extents, Vector3d normal, Point3d point)
    {
        var offsetVector = Offset * normal;
        point += offsetVector;
        var maxPoint = extents.MaxPoint + offsetVector;
        var minPoint = extents.MinPoint + offsetVector;
        var (startPoint, endPoint) = IsReverseDirection ? (minPoint, maxPoint) : (maxPoint, minPoint);

        // Вычисляем проекцию оси Z на нормаль: (Z · n) * n
        var projOnNormal = normal * Vector3d.ZAxis.DotProduct(normal);
        // Вычитаем из оси Z её проекцию на нормаль → получаем проекцию на плоскость
        var direction = (Vector3d.ZAxis - projOnNormal).GetNormal() * IsReverseDirection.GetSign();
        //wireDirection = new Vector3d(normal.Y, -normal.X, 0);

        startPoint = point.GetPoint(direction, startPoint.Z);
        endPoint = point.GetPoint(direction, endPoint.Z);

        var toolDirection = direction.GetPerpendicularVector();
        var approachPoint = startPoint - TechProcess.Approach * direction;
        if (approachPoint.Z > Processor.UpperZ)
            approachPoint = point.GetPoint(direction, Processor.UpperZ);

        Processor.Move(approachPoint, toolDirection, IsReverseAngle, IsReverseU);
        Processor.Cutting(startPoint, toolDirection);
        Processor.Cutting(endPoint, toolDirection);
        Processor.Cutting(endPoint + TechProcess.Departure * direction, toolDirection);
    }

    private void SurfaceCutting(List<Curve> railCurves)
    {
        foreach (var railCurve in railCurves.Where(p => p.StartPoint.Z < p.EndPoint.Z ^ IsReverseDirection))
            railCurve.ReverseCurve();

        var points = railCurves.ConvertAll(p => p.GetGeCurve().GetSamplePoints(StepCount).Select(x => x.Point).ToList());
        var directions = points[0].ConvertAll(p => railCurves[0].GetFirstDerivative(p).GetNormal());
        var offsets = points[0].Select((p, i) => directions[i].CrossProduct(points[1][i] - p).GetNormal() * Offset).ToArray();
        points = points.ConvertAll(ps => ps.Select((p, i) => p + offsets[i]).ToList());

        var approachPoints = points.ConvertAll(p =>
        {
            var pt = p[0] - TechProcess.Approach * directions[0];
            return pt.Z > Processor.UpperZ
                ? p[0].GetPoint(directions[0], Processor.UpperZ)
                : pt;
        });
        Processor.Move(approachPoints[0],approachPoints[1], IsReverseAngle, IsReverseU);
        for (var i = 0; i < points[0].Count; i++)
            Processor.Cutting(points[0][i], points[1][i]);

        var last = points[0].Count - 1;
        var departure = TechProcess.Departure * directions[last];
        Processor.Cutting(points[0][last] + departure, points[1][last] + departure);
    }
}