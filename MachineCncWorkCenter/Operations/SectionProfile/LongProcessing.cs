using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using CAM.Autocad;
using CAM.Autocad.AutoCADCommands;
using CAM.Core.Enums;
using CAM.Core.Processing;
using CAM.UI;
using CAM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.MachineCncWorkCenter.Operations.SectionProfile;

[Serializable]
public class LongProcessing: OperationCnc
{
    public AcadObject Rail { get; set; }
    public double? Length { get; set; }
    public double Departure { get; set; }

    public bool IsA90 { get; set; }
    public double Crest { get; set; } = 4;
    // public bool IsProfileStep { get; set; }
    public double? FirstPass { get; set; }
    public double? LastPass { get; set; }
    public bool IsReverse { get; set; }
    public double? PenetrationStep { get; set; }
    public double? PenetrationBegin { get; set; }
    public double? PenetrationMax { get; set; }
    public bool IsOutlet { get; set; } = true;
    public bool ChangeProcessSide { get; set; }
    public bool ChangeEngineSide { get; set; }

    // public bool IsExactlyBegin { get; set; }
    // public bool IsExactlyEnd { get; set; }
    // public double DzBillet { get; set; }

    public static void ConfigureParamsView(ParamsControl view)
    {
        view.AddAcadObject(nameof(Rail), "Направляющая");
        view.AddTextBox(nameof(Length), "Длина направляющей", hint: "Длина направляющей на оси Х");
        view.AddTextBox(nameof(Departure));
        view.AddIndent();
        view.AddAcadObject(nameof(ProcessingArea), "Профиль");
        var checkBoxIsA90 = view.AddCheckBox(nameof(IsA90), "A=90", "Признак обработки горизонтально расположенным диском");
        view.AddTextBox(nameof(Crest), "Толщина гребня минимум", hint: "Минимальная толщина материала между соседними пропилами", required: true);
        view.AddTextBox(nameof(FirstPass), "Начало профиля", hint: "Расстояние от начала профиля с которого начинается обработка");
        //view.AddCheckBox(nameof(IsExactlyBegin), "Начало точно", "Начало обработки с координаты при которой диск не выходит за границы профиля");
        view.AddTextBox(nameof(LastPass), "Конец профиля", hint: "Расстояние от начала профиля на котором заканчивается обработка");
        view.AddCheckBox(nameof(IsReverse), "Обратно");
        //view.AddCheckBox(nameof(IsExactlyEnd), "Конец точно", "Завершение обработки с координатой при которой диск не выходит за границы профиля");
        //view.AddTextBox(nameof(IsProfileStep), "Шаг по профилю");
        view.AddIndent();
        view.AddTextBox(nameof(PenetrationStep), "Шаг заглубления макс.", required: true, hint: "Наибольший шаг заглубления");
        view.AddTextBox(nameof(PenetrationBegin), "Заглубление: начало", hint: "Расстояние от края заготовки до ближайшего конца профиля");
        view.AddTextBox(nameof(PenetrationMax), "Заглубление максимум", hint: "Максимально возможное заглубление");
        view.AddCheckBox(nameof(IsOutlet), "Отвод");
        view.AddIndent();
        //view.AddTextBox(nameof(Delta));
        view.AddCheckBox(nameof(ChangeProcessSide), "Другая сторона", hint: "Сменить сторону обработки");
        var checkBoxChangeEngineSide = view.AddCheckBox(nameof(ChangeEngineSide), "Разворот двигателя на 180");
        //view.AddTextBox(nameof(DzBillet), "dZ заготовки");

        checkBoxIsA90.CheckedChanged += (object sender, EventArgs e) =>
        {
            checkBoxChangeEngineSide.Enabled = !checkBoxIsA90.Checked;
            if (checkBoxIsA90.Checked)
            {
                checkBoxChangeEngineSide.Checked = false;
                view.GetData<LongProcessing>().ChangeEngineSide = false;
            }
        };
    }

    public override void Execute()
    {
        if (Rail == null && Length == null)
            Length = 1000;

        var rail = Rail?.GetCurve() ?? new Line(Point3d.Origin, Point3d.Origin + Vector3d.XAxis * Length.Value);
        var profile = ProcessingArea.Get<Polyline>();       
        var railPoint = profile.StartPoint.Y <= profile.EndPoint.Y ? profile.StartPoint : profile.EndPoint;
        Processor.StartOperation(Math.Abs(profile.EndPoint.Y - profile.StartPoint.Y));

        var outside = ChangeProcessSide.GetSign();
        CreateProfile3D(profile, railPoint, rail, -outside);
        profile = profile.CreateCopy(Delta * (railPoint == profile.StartPoint).GetSign());
        if (Delta != 0)
            CreateProfile3D(profile, railPoint, rail, -outside);
        if (Departure != 0)
            rail = CreateDepartureRail(rail, Departure);

        if (profile.StartPoint.Y > profile.EndPoint.Y)
            profile.ReverseCurve();
        var offsetVector = profile.StartPoint - railPoint;
        if (IsA90)
            profile.TransformBy(Matrix3d.Rotation(-Math.PI / 2, Vector3d.ZAxis, profile.StartPoint));
        var points = profile.GetPolylineFitPoints(1D).Select(p => Point2d.Origin + (p - profile.StartPoint).ToVector2d()).ToArray();
        var (first, last) = (points.First(), points.Last());
        var distX = (LastPass.HasValue ? LastPass.Value - ToolThickness : last.X) - (FirstPass ?? -ToolThickness);
        var xStep = distX / (int)Math.Round(distX / (Crest + ToolThickness));
        var xStart = FirstPass ?? (xStep - ToolThickness);
        var xEnd = LastPass.HasValue ? LastPass.Value - ToolThickness : last.X - xStep;
        var yStart = (last.Y > first.Y ? last.Y - first.Y : 0) + PenetrationBegin.GetValueOrDefault();
        var index = 0;
        var cuts = Enumerable.Range(0, (int)Math.Round((xEnd - xStart) / xStep + 1))
            .Select(p =>
            {
                var x = xStart + p * xStep;
                (var y, index) = Helpers.FindMax(points, x, x + ToolThickness, index);
                return GetPenetrations(y).Select(y => CreateCuttingCurve(x + (ChangeEngineSide ? ToolThickness : 0), y)).ToList();
            });
        if (!IsReverse)
            cuts = cuts.Reverse();

        var outletCurve = (IsA90 && IsOutlet) ? rail.CreateCopy((yStart + TechProcess.ZSafety) * outside) : null;
        var engineSide = outside * ChangeEngineSide.GetSign();
        var angleA = IsA90 ? 90 : 0;
        foreach (var curves in cuts)
        {
            if (IsA90 && IsOutlet)
            {
                var point = Processor.GetClosestToolPoint(outletCurve);
                if (Processor.IsUpperTool)
                    Processor.Move(point, outletCurve.GetToolAngle(point, (Side)engineSide), angleA);
                Processor.Penetration(point.WithZ(curves[0].StartPoint.Z));

                curves.ForEach(p => Processor.Cutting(p, engineSide));
                Processor.Penetration(Processor.GetClosestToolPoint(outletCurve).WithZ(Processor.ToolPoint.Z));
            }
            else
            {
                curves.ForEach(p => Processor.Cutting(p, engineSide, angleA: angleA));
                if (IsOutlet)
                    Processor.Uplifting();
            }
        }
        Processor.Uplifting();

        return;

        IEnumerable<double> GetPenetrations(double y)
        {
            var penetrationAll = yStart - y;
            if (penetrationAll > PenetrationMax)
                penetrationAll = PenetrationMax.Value;
            var count = (int)Math.Ceiling(penetrationAll / PenetrationStep.Value);
            var step = penetrationAll / count;
            return Enumerable.Range(1, count).Select(p => yStart - p * step);
        }

        Curve CreateCuttingCurve(double x, double penetration)
        {
            var (offset, dz) = IsA90 ? (-penetration, x) : (x, penetration);
            var toolpath = rail.CreateCopy((offset + offsetVector.X) * -outside, dz + offsetVector.Y);
            //ProcessingObjectBuilder.AddEntity(toolpath);
            //ProcessingObjectBuilder.AddEntity(NoDraw.Line(toolpath.StartPoint,
            //    toolpath.StartPoint + (IsA90 ? Vector3d.ZAxis : Vector3d.YAxis) * ToolThickness));
            //ProcessingObjectBuilder.AddEntity(new Circle(toolpath.StartPoint, Vector3d.XAxis, 1));
            return toolpath;
        }
    }

    private void CreateProfile3D(Curve profile, Point3d profilePoint, Curve rail, int sign)
    {
        CreateProfileAtPoint(rail.StartPoint);
        CreateProfileAtPoint(rail.EndPoint);

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
