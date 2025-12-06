using Autodesk.AutoCAD.Geometry;
using CAM.Autocad;
using CAM.Autocad.AutoCADCommands;
using CAM.UI;
using CAM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.MachineCncWorkCenter.Operations.SectionProfile;

[Serializable]
public class LongCleaning : SectionProfileBase
{
    [NonSerialized] private List<double> _anglesA;
    public double Step { get; set; } = 1;
    public bool IsProfileStep { get; set; }
    public bool IsCalcAngleA { get; set; }
    public bool IsExactlyBegin { get; set; }
    public bool IsExactlyEnd { get; set; }

    public static void ConfigureParamsView(ParamsControl view)
    {
        ConfigureParamsViewBase(view);

        view.AddIndent();
        view.AddTextBox(nameof(Step), required: true);
        view.AddCheckBox(nameof(IsProfileStep), "Шаг по профилю");
        var checkBoxIsCalcAngleA = view.AddCheckBox(nameof(IsCalcAngleA), "Угол А расчет");
        view.AddCheckBox(nameof(IsExactlyBegin), "Начало точно", "Начало обработки с координаты при которой диск не выходит за границы профиля");
        view.AddCheckBox(nameof(IsExactlyEnd), "Конец точно", "Завершение обработки с координатой при которой диск не выходит за границы профиля");

        AngleATextBox.Validated += (_, _) => UpdateCheckBoxEnable(checkBoxIsCalcAngleA);
    }

    public override void Execute()
    {
        base.Execute();

        _anglesA = null;
        var toolPoints = (IsCalcAngleA
                ? GetToolPointsCalcA()
                : GetToolPoints())
            .If(ProfileStart.HasValue, x => x.Where(p => p.X > ProfileStart || p.X.IsEqual(ProfileStart.Value)))
            .If(ProfileEnd.HasValue, x => x.Where(p => p.X < ProfileEnd || p.X.IsEqual(ProfileEnd.Value)))
            .If(!IsReverse, p => p.Reverse());
        CreateCurves(toolPoints)
            .Select((p, i) => new { Curve = p, AngleA = _anglesA != null ? _anglesA[i] : AngleA })
            .ForEach(p => Processor.Cutting(p.Curve, _engineSide, angleA: p.AngleA));
        Processor.Uplifting();
        _profile.Dispose();
    }

    private IEnumerable<Point2d> GetToolPoints()
    {
        var polylinePoints = _profile.GetPolylineFitPoints(1D).Select(p => p.ToPoint2d()).ToArray();
        var toolZeroShiftVector = Vector2d.XAxis * (ChangeEngineSide ? ToolThickness : 0);
        return (IsProfileStep
                ? _profile.GetPointsByDist(Step).Select(p => p.X - (_profile.StartPoint.Y <= _profile.EndPoint.Y ? ToolThickness : 0))
                : GetXs(_profile.EndPoint.X))
            .FindMax(polylinePoints, ToolThickness)
            .Select(p => p + toolZeroShiftVector);

        IEnumerable<double> GetXs(double profileEnd)
        {
            var xStart = ProfileStart ?? 0;
            if (!IsExactlyBegin)
                xStart -= ToolThickness - Step;
            var xEnd = (ProfileEnd ?? profileEnd) - ToolThickness;
            if (!IsExactlyEnd)
                xEnd += ToolThickness - Step;
            var distX = xEnd - xStart;
            var count = (int)Math.Round(distX / Step);
            var xStep = distX / count;

            return Enumerable.Range(0, count + 1).Select(p => xStart + p * xStep);
        }
    }

    private IEnumerable<Point2d> GetToolPointsCalcA()
    {
        var angles = new List<double>();
        var points = new List<Point2d>();
        var par = 1;
        var vertexDist = _profile.GetDistanceAtParameter(par);
        for (var dist = 0d; dist <= _profile.GetDistanceAtParameter(_profile.EndParam); dist += Step)
        {
            var point = _profile.GetPointAtDistX(dist);
            var tangent = _profile.GetTangent(point).GetNormal();

            if (dist >= vertexDist - ToolThickness / 2 && par < _profile.NumberOfVertices - 1)
            {
                var vertexTangent = _profile.GetFirstDerivative(par).ToVector2d();
                if (!tangent.IsTurnRight(vertexTangent))
                {
                    var pt = _profile.GetPoint2dAt(par);
                    var tg = _profile.GetFirstDerivative(par - 0.0001).ToVector2d().GetNormal();
                    points.Add(pt - tg * ToolThickness);
                    angles.Add(tg.Angle.ToDeg());
                    points.Add(pt);
                    angles.Add(vertexTangent.Angle.ToDeg());
                    dist = vertexDist + ToolThickness / 2;
                    vertexDist = _profile.GetDistanceAtParameter(++par);
                    continue;
                }

                vertexDist = _profile.GetDistanceAtParameter(++par);
            }

            points.Add(point.ToPoint2d() - tangent * ToolThickness / 2);
            angles.Add(tangent.Angle.ToDeg());
            //ProcessingObjectBuilder.AddEntity(new Circle(rail.StartPoint - (point.X + offsetVector.X) * Vector3d.YAxis + point.Y * Vector3d.ZAxis, Vector3d.XAxis, 0.2));
        }

        _anglesA = angles.ConvertAll(p => p <= 90 ? p : 0);
        return points;
    }
}
