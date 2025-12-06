using Autodesk.AutoCAD.DatabaseServices;
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
public class LongProcessing: SectionProfileBase
{
    //public AcadObject Rail { get; set; }
    //public double? Length { get; set; }
    //public double Departure { get; set; }

    //public bool IsA90 { get; set; }
    //public double AngleA { get; set; }

    public double Crest { get; set; } = 4;
    // public bool IsProfileStep { get; set; }
    //public double? ProfileStart { get; set; }
    //public double? ProfileEnd { get; set; }
    //public bool IsReverse { get; set; }
    public double? PenetrationStep { get; set; }
    public double? PenetrationBegin { get; set; }
    public double? PenetrationMax { get; set; }
    public bool IsOutlet { get; set; } = true;
    //public bool ChangeProcessSide { get; set; }
    //public bool ChangeEngineSide { get; set; }

    // public bool IsExactlyBegin { get; set; }
    // public bool IsExactlyEnd { get; set; }
    // public double DzBillet { get; set; }

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
        // var checkBoxIsA90 = view.AddCheckBox(nameof(IsA90), "A=90", "Признак обработки горизонтально расположенным диском");
        var angleATextBox = view.AddTextBox(nameof(AngleA), "Угол A", hint: "Вертикальный угол");
        view.AddTextBox(nameof(Crest), "Толщина гребня минимум", hint: "Минимальная толщина материала между соседними пропилами", required: true);
        view.AddTextBox(nameof(ProfileStart), "Начало профиля", hint: "Расстояние от начала профиля с которого начинается обработка");
        //view.AddCheckBox(nameof(IsExactlyBegin), "Начало точно", "Начало обработки с координаты при которой диск не выходит за границы профиля");
        view.AddTextBox(nameof(ProfileEnd), "Конец профиля", hint: "Расстояние от начала профиля на котором заканчивается обработка");
        view.AddCheckBox(nameof(IsReverse), "Обратно", hint: "Обратное направление обработки профиля");
        //view.AddCheckBox(nameof(IsExactlyEnd), "Конец точно", "Завершение обработки с координатой при которой диск не выходит за границы профиля");
        //view.AddTextBox(nameof(IsProfileStep), "Шаг по профилю");
        view.AddIndent();
        view.AddTextBox(nameof(PenetrationStep), "Шаг заглубления макс.", required: true, hint: "Наибольший шаг заглубления");
        view.AddTextBox(nameof(PenetrationBegin), "Заглубление: начало", hint: "Расстояние от края заготовки до ближайшего конца профиля");
        view.AddTextBox(nameof(PenetrationMax), "Заглубление максимум", hint: "Максимально возможное заглубление");
        view.AddCheckBox(nameof(IsOutlet), "Отвод");
        view.AddIndent();
        view.AddCheckBox(nameof(ChangeProcessSide), "Другая сторона", hint: "Сменить сторону обработки");
        var checkBoxChangeEngineSide = view.AddCheckBox(nameof(ChangeEngineSide), "Разворот двигателя на 180");

        //checkBoxIsA90.CheckedChanged += (object sender, EventArgs e) =>
        //{
        //    checkBoxChangeEngineSide.Enabled = !checkBoxIsA90.Checked;
        //    if (checkBoxIsA90.Checked)
        //    {
        //        checkBoxChangeEngineSide.Checked = false;
        //        view.GetData<LongProcessing>().ChangeEngineSide = false;
        //    }
        //};
    }

    public override void Execute()
    {
        base.Execute();

        //if (Rail == null && Length == null)
        //    Length = 1000;

        //var rail = Rail?.GetCurve() ?? new Line(Point3d.Origin, Point3d.Origin + Vector3d.XAxis * Length.Value);
        //var profile = ProcessingArea.Get<Polyline>();
        //var railPoint = profile.StartPoint.Y <= profile.EndPoint.Y ? profile.StartPoint : profile.EndPoint;
        //Processor.StartOperation(Math.Abs(profile.EndPoint.Y - profile.StartPoint.Y));

        //var outside = ChangeProcessSide.GetSign();
        //var engineSide =
        //    outside * ChangeEngineSide
        //        .GetSign(); // по-умолчанию двигатель со стороны заготовки, обратно наружней стороне
        //CreateProfile3D(profile, railPoint, rail, -outside);
        //profile = profile.CreateCopy(Delta * (railPoint == profile.StartPoint).GetSign());
        //if (Delta != 0)
        //    CreateProfile3D(profile, railPoint, rail, -outside);
        //if (Departure != 0)
        //    rail = CreateDepartureRail(rail, Departure);

        //if (profile.StartPoint.Y > profile.EndPoint.Y)
        //    profile.ReverseCurve();
        //var offsetVector = (profile.StartPoint - railPoint).ToVector2d();
        //profile.TransformBy(Matrix3d.Displacement(Point3d.Origin - profile.StartPoint));
        //if (AngleA > 0)
        //    profile.TransformBy(Matrix3d.Rotation(-AngleA.ToRad(), Vector3d.ZAxis, profile.StartPoint));
        var polylinePoints = profile.GetPolylineFitPoints(1D).Select(p => p.ToPoint2d()).ToArray();
        var xs = GetX(profile.EndPoint.X);
        var yStart = (profile.EndPoint.Y > 0 ? profile.EndPoint.Y : 0) +
                     (AngleA > 0 ? PenetrationBegin.GetValueOrDefault() : 0);
        var xShift = ChangeEngineSide ? ToolThickness : 0;
        var cuts = xs.FindMax(polylinePoints, ToolThickness)
            .Select(p => GetToolPointsList(p, yStart, xShift))
            .If(!IsReverse, p => p.Reverse())
            .Select(p => CreateCurves(p).ToList());
        //if (AngleA > 0)
        //    cuts = cuts.Select(c =>
        //        c.ConvertAll(p => p.TransformBy(Matrix2d.Rotation(AngleA.ToRad(), Point2d.Origin))));
        //if (!IsReverse)
        //    cuts = cuts.Reverse();

        foreach (var curves in cuts)
        {
            //var curves = CreateCurves(cut).ToList();
                //cut.Points.ConvertAll(p => p + offsetVector)
                //.ConvertAll(p => rail.CreateCopy(-outside * p.X, p.Y));
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
                Processor.Penetration(Processor.GetClosestToolPoint(first));
        }

        Processor.Uplifting();


        //IEnumerable<Curve> CreateCurves(IEnumerable<Point2d> points)
        //{
        //    return points.If(AngleA > 0,
        //            p => p.Select(p => p.TransformBy(Matrix2d.Rotation(AngleA.ToRad(), Point2d.Origin))))
        //        .Select(p => p + offsetVector)
        //        .Select(p => rail.CreateCopy(-outside * p.X, p.Y));
        //}
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

    private IEnumerable<double> GetX(double profileEnd)
    {
        var distX = (ProfileEnd.HasValue ? ProfileEnd.Value - ToolThickness : profileEnd) - (ProfileStart ?? -ToolThickness);
        var xStep = distX / (int)Math.Round(distX / (Crest + ToolThickness));
        var xStart = ProfileStart ?? (xStep - ToolThickness);
        var xEnd = ProfileEnd.HasValue ? ProfileEnd.Value - ToolThickness : profileEnd - xStep;
        var count = (int)Math.Round((xEnd - xStart) / xStep);
        return Enumerable.Range(0, count + 1).Select(p => xStart + p * xStep);
    }

    //private void CreateProfile3D(Curve profile, Point3d profilePoint, Curve rail, int sign)
    //{
    //    CreateProfileAtPoint(rail.StartPoint);
    //    CreateProfileAtPoint(rail.EndPoint);

    //    void CreateProfileAtPoint(Point3d railPoint)
    //    {
    //        var angle = rail.GetTangent(railPoint).Angle + Math.PI / 2 * sign;
    //        var matrix = Matrix3d.Displacement(railPoint - profilePoint) *
    //            Matrix3d.Rotation(angle, Vector3d.ZAxis, profilePoint) *
    //            Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, profilePoint);
    //        var p = profile.GetTransformedCopy(matrix);
    //        p.Transparency = Acad.GetSemitransparent();
    //        ProcessingObjectBuilder.AddEntity(p);
    //    }
    //}

    //private Curve CreateDepartureRail(Curve curve, double departure)
    //{
    //    if (curve is Line line)
    //        return new Line(line.ExpandStart(departure), line.ExpandEnd(departure));
        
    //    var polyline = new Polyline();
    //    var startPoint = curve.StartPoint - curve.GetFirstDerivative(curve.StartPoint).GetNormal() * departure;
    //    polyline.AddVertexAt(0, startPoint.ToPoint2d(), 0, 0, 0);

    //    switch (curve)
    //    {
    //        case Polyline poly:
    //            polyline.JoinPolyline(poly);
    //            break;
    //        case Arc arc:
    //            var bulge = arc.GetArcBulge(curve.StartPoint);
    //            polyline.AddVertexAt(1, curve.StartPoint.ToPoint2d(), bulge, 0, 0);
    //            polyline.AddVertexAt(2, curve.EndPoint.ToPoint2d(), 0, 0, 0);
    //            break;
    //        default:
    //            throw new Exception($"Тип кривой {curve.GetType().Name} не поддерживается");
    //    }

    //    var endPoint = curve.EndPoint + curve.GetFirstDerivative(curve.EndPoint).GetNormal() * departure;
    //    polyline.AddVertexAt(polyline.GetPolyPoints().Count(), endPoint.ToPoint2d(), 0, 0, 0);

    //    return polyline;
    //}
}