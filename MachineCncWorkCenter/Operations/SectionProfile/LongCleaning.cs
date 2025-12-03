using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Autocad;
using CAM.Autocad.AutoCADCommands;
using CAM.Core.Processing;
using CAM.UI;
using CAM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.MachineCncWorkCenter.Operations.SectionProfile;

[Serializable]
public class LongCleaning : OperationCnc
{
    public AcadObject Rail { get; set; }
    public double? Length { get; set; }
    public double Departure { get; set; }

    public double AngleA { get; set; }
    public bool IsProfileStep { get; set; }
    public bool IsCalcAngleA { get; set; }
    public double Step { get; set; } = 1;
    public double? ProfileStart { get; set; }
    public bool IsExactlyBegin { get; set; }
    public double? ProfileEnd { get; set; }
    public bool IsExactlyEnd { get; set; }
    public bool IsReverse { get; set; }
    public bool ChangeProcessSide { get; set; }
    public bool ChangeEngineSide { get; set; }

    public static void ConfigureParamsView(ParamsControl view)
    {
        view.AddAcadObject(nameof(Rail), "Направляющая");
        view.AddTextBox(nameof(Length), "Длина направляющей", hint: "Длина направляющей на оси Х");
        view.AddTextBox(nameof(Departure));
        view.AddIndent();
        view.AddAcadObject(nameof(ProcessingArea), "Профиль");
        var angleATextBox = view.AddTextBox(nameof(AngleA), "Угол A", hint: "Вертикальный угол");
        view.AddCheckBox(nameof(ChangeProcessSide), "Другая сторона", hint: "Сменить сторону обработки");
        var checkBoxChangeEngineSide = view.AddCheckBox(nameof(ChangeEngineSide), "Разворот двигателя на 180");
        view.AddIndent();
        view.AddTextBox(nameof(ProfileStart), "Начало профиля",
            hint: "Расстояние от начала профиля до начала обрабатываемого участка профиля");
        view.AddCheckBox(nameof(IsExactlyBegin), "Начало точно",
            "Начало обработки с координаты при которой диск не выходит за границы профиля");
        view.AddTextBox(nameof(ProfileEnd), "Конец профиля",
            hint: "Расстояние от начала профиля до конца обрабатываемого участка профиля");
        view.AddCheckBox(nameof(IsExactlyEnd), "Конец точно",
            "Завершение обработки с координатой при которой диск не выходит за границы профиля");
        view.AddCheckBox(nameof(IsReverse), "Обратно", hint: "Обратное направление обработки профиля");
        view.AddTextBox(nameof(Step), required: true);
        view.AddCheckBox(nameof(IsProfileStep), "Шаг по профилю");
        var checkBoxIsCalcAngleA = view.AddCheckBox(nameof(IsCalcAngleA), "Угол А расчет");

        angleATextBox.Validated += (_, _) => SetCheckBoxChangeEngineSideEnable(angleATextBox.Text == "0");
        checkBoxIsCalcAngleA.CheckedChanged += (_, _) => SetCheckBoxChangeEngineSideEnable(!checkBoxIsCalcAngleA.Checked);

        return;

        void SetCheckBoxChangeEngineSideEnable(bool enabled)
        {
            checkBoxChangeEngineSide.Enabled = enabled;
            if (!enabled)
            {
                checkBoxChangeEngineSide.Checked =  false;
                view.GetData<LongCleaning>().ChangeEngineSide = false;
            }
        }
    }

    public override void Execute()
    {
        if (Rail == null && Length == null)
            Length = 1000;

        var rail = Rail?.GetCurve() ?? new Line(Point3d.Origin, Point3d.Origin + Vector3d.XAxis * Length.Value);
        var profile = ProcessingArea.Get<Polyline>();
        var railPoint = profile.StartPoint.Y <= profile.EndPoint.Y ? profile.StartPoint : profile.EndPoint;
        Processor.StartOperation(Math.Abs(profile.EndPoint.Y - profile.StartPoint.Y));

        var outside = ChangeProcessSide.GetSign(); // по-умолчанию наружная сторона справа
        CreateProfile3D(profile, railPoint, rail, -outside);
        profile = profile.CreateCopy(Delta * (railPoint == profile.StartPoint).GetSign());
        if (Delta != 0)
            CreateProfile3D(profile, railPoint, rail, -outside);
        if (Departure != 0)
            rail = CreateDepartureRail(rail, Departure);

        if (profile.StartPoint.Y > profile.EndPoint.Y)
            profile.ReverseCurve();
        var offsetVector = (profile.StartPoint - railPoint).ToVector2d();
        profile.TransformBy(Matrix3d.Displacement(Point3d.Origin - profile.StartPoint));
        if (AngleA > 0)
            profile.TransformBy(Matrix3d.Rotation(-AngleA.ToRad(), Vector3d.ZAxis, profile.StartPoint));

        var (toolPoints, angles) = IsCalcAngleA ? GetToolPointsCalcA(profile) : (GetToolPoints(profile), null);
        if (ProfileStart.HasValue)
            toolPoints = toolPoints.Where(p => p.X > ProfileStart || p.X.IsEqual(ProfileStart.Value));
        if (ProfileEnd.HasValue)
            toolPoints = toolPoints.Where(p => p.X < ProfileEnd || p.X.IsEqual(ProfileEnd.Value));

        if (!IsReverse)
            toolPoints = toolPoints.Reverse();
        var curves = toolPoints.Select(p => p + offsetVector)
            .Select(p => rail.CreateCopy(-outside * p.X, p.Y));
        var engineSide = outside * ChangeEngineSide.GetSign(); // по-умолчанию двигатель со стороны заготовки, обратно наружней стороне
        curves.Select((p, i) => new { Curve = p, AngleA = angles != null ? angles[i] : AngleA })
            .ForEach(p => Processor.Cutting(p.Curve, engineSide, angleA: p.AngleA));
        Processor.Uplifting();
    }

    private IEnumerable<Point2d> GetToolPoints(Polyline profile)
    {
        var polylinePoints = profile.GetPolylineFitPoints(1D).Select(p => p.ToPoint2d()).ToArray();
        var xs = IsProfileStep
            ? profile.GetPointsByDist(Step).Select(p => p.X - (profile.StartPoint.Y <= profile.EndPoint.Y ? ToolThickness : 0))
            : GetXsByAxis(profile.EndPoint.X);
        var toolZeroShiftVector = Vector2d.XAxis * (ChangeEngineSide ? ToolThickness : 0);
        var toolPoints = xs.FindMax(polylinePoints, ToolThickness)
            .Select(p => p + toolZeroShiftVector);
        if (AngleA > 0)
            toolPoints = toolPoints.Select(p => p.TransformBy(Matrix2d.Rotation(AngleA.ToRad(), Point2d.Origin)));
        return toolPoints;
    }

    private IEnumerable<double> GetXsByAxis(double profileEnd)
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

    private (IEnumerable<Point2d> points, List<double>) GetToolPointsCalcA(Polyline profile)
    {
        var angles = new List<double>();
        var points = new List<Point2d>();
        var par = 1;
        var vertexDist = profile.GetDistanceAtParameter(par);
        for (var dist = 0d; dist <= profile.GetDistanceAtParameter(profile.EndParam); dist += Step)
        {
            var point = profile.GetPointAtDistX(dist);
            var tangent = profile.GetTangent(point).GetNormal();

            if (dist >= vertexDist - ToolThickness / 2 && par < profile.NumberOfVertices - 1)
            {
                var vertexTangent = profile.GetFirstDerivative(par).ToVector2d();
                if (!tangent.IsTurnRight(vertexTangent))
                {
                    var pt = profile.GetPoint2dAt(par);
                    var tg = profile.GetFirstDerivative(par - 0.0001).ToVector2d().GetNormal();
                    points.Add(pt - tg * ToolThickness);
                    angles.Add(tg.Angle.ToDeg());
                    points.Add(pt);
                    angles.Add(vertexTangent.Angle.ToDeg());
                    dist = vertexDist + ToolThickness / 2;
                    vertexDist = profile.GetDistanceAtParameter(++par);
                    continue;
                }

                vertexDist = profile.GetDistanceAtParameter(++par);
            }

            points.Add(point.ToPoint2d() - tangent * ToolThickness / 2);
            angles.Add(tangent.Angle.ToDeg());
            //ProcessingObjectBuilder.AddEntity(new Circle(rail.StartPoint - (point.X + offsetVector.X) * Vector3d.YAxis + point.Y * Vector3d.ZAxis, Vector3d.XAxis, 0.2));
        }

        return (points, angles.ConvertAll(p => p <= 90 ? p : 0));
    }

    private void CreateProfile3D(Curve profile, Point3d profilePoint, Curve rail, int sign)
    {
        CreateProfileAtPoint(rail.StartPoint);
        CreateProfileAtPoint(rail.EndPoint);
        return;

        void CreateProfileAtPoint(Point3d railPoint)
        {
            var angle = rail.GetTangent(railPoint).Angle + Math.PI / 2 * sign;
            var matrix = Matrix3d.Displacement(railPoint - profilePoint) *
                Matrix3d.Rotation(angle, Vector3d.ZAxis, profilePoint) *
                Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, profilePoint);
            var p = profile.GetTransformedCopy(matrix);
            p.Transparency = Acad.GetSemitransparent();
            ProcessingObjectBuilder.AddEntity(p);
        }
    }

    private Curve CreateDepartureRail(Curve curve, double departure)
    {
        if (curve is Line line)
            return new Line(line.ExpandStart(departure), line.ExpandEnd(departure));

        var polyline = new Polyline();
        var startPoint = curve.StartPoint - curve.GetFirstDerivative(curve.StartPoint).GetNormal() * departure;
        polyline.AddVertexAt(0, startPoint.ToPoint2d(), 0, 0, 0);

        switch (curve)
        {
            case Polyline poly:
                polyline.JoinPolyline(poly);
                break;
            case Arc arc:
                var bulge = arc.GetArcBulge(curve.StartPoint);
                polyline.AddVertexAt(1, curve.StartPoint.ToPoint2d(), bulge, 0, 0);
                polyline.AddVertexAt(2, curve.EndPoint.ToPoint2d(), 0, 0, 0);
                break;
            default:
                throw new Exception($"Тип кривой {curve.GetType().Name} не поддерживается");
        }

        var endPoint = curve.EndPoint + curve.GetFirstDerivative(curve.EndPoint).GetNormal() * departure;
        polyline.AddVertexAt(polyline.GetPolyPoints().Count(), endPoint.ToPoint2d(), 0, 0, 0);

        return polyline;
    }
}
