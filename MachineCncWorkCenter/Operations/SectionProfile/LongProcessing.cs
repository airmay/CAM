using Autodesk.AutoCAD.Geometry;
using CAM.Autocad;
using CAM.Autocad.AutoCADCommands;
using CAM.Core.Enums;
using CAM.UI;
using CAM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.MachineCncWorkCenter.Operations.SectionProfile;

[Serializable]
public class LongProcessing: SectionProfileBase
{
    public double? PenetrationBegin { get; set; }
    public double? PenetrationMax { get; set; }
    public double? PenetrationStep { get; set; }
    public double Crest { get; set; } = 4;
    public bool IsOutlet { get; set; } = true;

    public static void ConfigureParamsView(ParamsControl view)
    {
        ConfigureParamsViewBase(view);

        view.AddIndent();
        view.AddTextBox(nameof(PenetrationBegin), "Заглубление: начало", hint: "Расстояние от края заготовки до ближайшего конца профиля");
        view.AddTextBox(nameof(PenetrationMax), "Заглубление максимум", hint: "Максимально возможное заглубление");
        view.AddTextBox(nameof(PenetrationStep), "Шаг заглубления макс.", required: true, hint: "Наибольший шаг заглубления");
        view.AddTextBox(nameof(Crest), "Толщина гребня минимум", hint: "Минимальная толщина материала между соседними пропилами", required: true);
        view.AddCheckBox(nameof(IsOutlet), "Отвод");
    }

    public override void Execute()
    {
        base.Execute();

        var polylinePoints = _profile.GetPolylineFitPoints(1D).Select(p => p.ToPoint2d()).ToArray();
        var xs = GetXs(_profile.EndPoint.X);
        var yStart = (_profile.EndPoint.Y > 0 ? _profile.EndPoint.Y : 0) + (AngleA > 0 ? PenetrationBegin.GetValueOrDefault() : 0);
        var xShift = ChangeEngineSide ? ToolThickness : 0;

        var cuts = xs.FindMax(polylinePoints, ToolThickness)
            .Select(p => GetToolPointsList(p, yStart, xShift))
            .If(!IsReverse, p => p.Reverse())
            .Select(p => CreateCurves(p).ToList());

        foreach (var curves in cuts)
        {
            var first = curves[0];
            if (IsOutlet)
            {
                var point = Processor.GetClosestToolPoint(first);
                if (Processor.IsUpperTool)
                    Processor.Move(point, first.GetToolAngle(point, (Side)_engineSide), AngleA);
                Processor.Penetration(point);
                curves.RemoveAt(0);
            }

            Processor.Cutting(curves, _engineSide, angleA: AngleA);

            if (IsOutlet)
            {
                Processor.GCommandTo(0, Processor.GetClosestToolPoint(first));
                first.Dispose();
            }
        }

        Processor.Uplifting();
        DisposeCurves();
    }

    private IEnumerable<double> GetXs(double profileEnd)
    {
        var distX = (ProfileEnd.HasValue ? ProfileEnd.Value - ToolThickness : profileEnd) - (ProfileStart ?? -ToolThickness);
        var xStep = distX / (int)Math.Round(distX / (Crest + ToolThickness));
        var xStart = ProfileStart ?? (xStep - ToolThickness);
        var xEnd = ProfileEnd.HasValue ? ProfileEnd.Value - ToolThickness : profileEnd - xStep;
        var count = (int)Math.Round((xEnd - xStart) / xStep);
        return Enumerable.Range(0, count + 1).Select(p => xStart + p * xStep);
    }

    private List<Point2d> GetToolPointsList(Point2d point, double yStart, double xShift)
    {
        var penetrationAll = yStart - point.Y;
        if (penetrationAll > PenetrationMax)
            penetrationAll = PenetrationMax.Value;
        var count = (int)Math.Ceiling(penetrationAll / PenetrationStep.Value);
        var step = penetrationAll / count;
        var ys = Enumerable.Range(1, count).Select(p => yStart - p * step).ToList();
        if (IsOutlet)
            ys.Insert(0, yStart + TechProcess.ZSafety);
        return ys.ConvertAll(y => new Point2d(point.X + xShift, y));
    }
}