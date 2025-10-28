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
using System.Drawing.Drawing2D;
using System.Linq;

namespace CAM.MachineCncWorkCenter.Operations.SectionProfile;

[Serializable]
public class LongProcessing: OperationCnc
{
    public AcadObject Rail { get; set; }
    public double? Length { get; set; }
    public double? StepPass { get; set; }
    // public bool IsProfileStep { get; set; }
    public double? FirstPass { get; set; }
    public double? LastPass { get; set; }
    public double? PenetrationStep { get; set; }
    public double? PenetrationBegin { get; set; }
    public double? PenetrationMax { get; set; }
    // public double Delta { get; set; }
    // public int CuttingFeed { get; set; } = 5000;
    public double Departure { get; set; }
    // public bool IsOutlet { get; set; } = true;
    // public AcadObject Profile { get; set; }
    public bool IsA90 { get; set; }
    public bool ChangeProcessSide { get; set; }
    // public bool ChangeEngineSide { get; set; }
    // public bool IsExactlyBegin { get; set; }
    // public bool IsExactlyEnd { get; set; }
    // public double DzBillet { get; set; }

    public static void ConfigureParamsView(ParamsControl view)
    {
        view.AddAcadObject(nameof(ProcessingArea), "Профиль");
        view.AddAcadObject(nameof(Rail), "Направляющая");
        view.AddTextBox(nameof(Length), "Длина направляющей", hint: "Длина направляющей на оси Х");
        view.AddIndent();
        view.AddCheckBox(nameof(IsA90), "A=90", "Признак обработки горизонтально расположенным диском");
        view.AddTextBox(nameof(StepPass), required: true);
        view.AddTextBox(nameof(FirstPass), "Начало профиля", hint: "Расстояние от начала профиля с которого начинается обработка");
        //view.AddCheckBox(nameof(IsExactlyBegin), "Начало точно", "Начало обработки с координаты при которой диск не выходит за границы профиля");
        view.AddTextBox(nameof(LastPass), "Конец профиля", hint: "Расстояние от начала профиля на котором заканчивается обработка");
        //view.AddCheckBox(nameof(IsExactlyEnd), "Конец точно", "Завершение обработки с координатой при которой диск не выходит за границы профиля");
        //view.AddTextBox(nameof(IsProfileStep), "Шаг по профилю");
        view.AddIndent();
        view.AddTextBox(nameof(PenetrationStep), "Заглубление: шаг", required: true);
        view.AddTextBox(nameof(PenetrationBegin), "Заглубление: начало", hint: "Расстояние края заготовки до ближайшей точки профиля");
        view.AddTextBox(nameof(PenetrationMax), "Заглубление максимум", hint: "Максимально возможное заглубление");
        view.AddIndent();
        //view.AddCheckBox(nameof(IsOutlet), "Отвод");
        view.AddTextBox(nameof(Departure));
        //view.AddTextBox(nameof(Delta));
        view.AddCheckBox(nameof(ChangeProcessSide), "Сторона обработки");
        //view.AddCheckBox(nameof(ChangeEngineSide), "Разворот двигателя на 180");
        //view.AddTextBox(nameof(DzBillet), "dZ заготовки");
    }

    public override void Execute()
    {
        if (Rail == null && Length == null)
            Length = 1000;

        var railBase = Rail?.GetCurve() ?? new Line(Point3d.Origin, Point3d.Origin + Vector3d.XAxis * Length.Value);
        var profile = ProcessingArea.Get<Polyline>();
        Processor.StartOperation(Math.Abs(profile.EndPoint.Y - profile.StartPoint.Y));
        var baseRailPoint = profile.StartPoint.Y <= profile.EndPoint.Y ? profile.StartPoint : profile.EndPoint;

        var processSide = ChangeProcessSide ? -1 : 1;
        CreateProfile3D(profile, baseRailPoint, railBase, railBase.StartPoint);
        CreateProfile3D(profile, baseRailPoint, railBase, railBase.EndPoint);

        var rail = CreateDepartureRail(railBase, Departure);

        profile = (Polyline)profile.GetOffsetCurves(-Delta)[0];
        if (Delta != 0)
        {
            CreateProfile3D(profile, baseRailPoint, railBase, railBase.StartPoint);
            CreateProfile3D(profile, baseRailPoint, railBase, railBase.EndPoint);
        }

        //if (!(railBase is Line))
        //    processSide *= -1;
        if (railBase.IsNewObject)
            railBase.Dispose();

        var profileWidth = Math.Abs(profile.EndPoint.X - profile.StartPoint.X);
        var profileHeight = Math.Abs(profile.EndPoint.Y - profile.StartPoint.Y);
        var railPoint = profile.StartPoint.Y <= profile.EndPoint.Y ? profile.StartPoint : profile.EndPoint;
        var offsetVector = railPoint - baseRailPoint;
        var points = profile.GetPolylineFitPoints(1D).ToArray();

        if (!IsA90)
        {
            if (points.First().X < points.Last().X)
                points = points.Reverse().ToArray();

            var width = points.First().X - points.Last().X;
            var height = points.First().Y - points.Last().Y + PenetrationBegin.GetValueOrDefault();
            var step = StepPass.Value;

            var x = FirstPass.HasValue ? width - FirstPass.Value : step;
            var xEnd = LastPass.HasValue ? width - LastPass.Value : width - ToolThickness;

            var xArray = points.ConvertAll(p => points.First().X - p.X);
            var yArray = points.ConvertAll(p => p.Y - railPoint.Y);
            var index = 0;
            var yMax = Math.Abs(profile.StartPoint.Y - profile.EndPoint.Y);
            //var allPenetration = PenetrationBegin.GetValueOrDefault() + yMax;
            //ProcessingObjectBuilder.AddEntity(profile);
            do
            {
                (var y, index) = Helpers.FindMax(xArray, yArray, x - ToolThickness, x, index);
                // ProcessingObjectBuilder.AddEntity(new Circle(new Point3d(profile.StartPoint.X - x, profile.EndPoint.Y + y, 0), Vector3d.ZAxis, 1));
                // ProcessingObjectBuilder.AddEntity(new Circle(new Point3d(profile.StartPoint.X - (x-ToolThickness), profile.EndPoint.Y + y, 0), Vector3d.ZAxis, 1));
                var penetration = height - y;
                var count = (int)Math.Ceiling(penetration / PenetrationStep.Value);
                var penetrationStepCalc = penetration / count;

                for (var i = count - 1; i >= 0; i--)
                {
                    var offset = width - x;
                    var dz = y + i * penetrationStepCalc;
                    var toolpath = CreateCopy(rail, offset + offsetVector.X, dz + offsetVector.Y);
                    ProcessingObjectBuilder.AddEntity(toolpath);
                    ProcessingObjectBuilder.AddEntity(NoDraw.Line(toolpath.StartPoint,
                        toolpath.StartPoint + (IsA90 ? Vector3d.ZAxis : Vector3d.YAxis) * ToolThickness));
                    ProcessingObjectBuilder.AddEntity(new Circle(toolpath.StartPoint, Vector3d.XAxis, 1));
                }
                //ProcessingObjectBuilder.AddEntity(NoDraw.Line(toolpath.StartPoint, toolpath.StartPoint + (IsA90 ? -Vector3d.YAxis : Vector3d.ZAxis) * 100));
                //if (Processor.IsUpperTool)
                //{
                //    var angleC = rail.GetToolAngle(rail.StartPoint, (Side)side);
                //    Processor.Move(tp[0].StartPoint, angleC, 90);
                //}

                //tp.ForEach(p => Cutting(p, fromStart = !fromStart, (Side)side));
                x += step;
            } while (x < xEnd);
        }
        else
        {
            if (points.First().Y < points.Last().Y)
                points = points.Reverse().ToArray();

            var step = StepPass.Value;
            var y = FirstPass.HasValue ? profileHeight - FirstPass.Value : step;
            var yEnd = LastPass.HasValue ? profileHeight - LastPass.Value : profileHeight - ToolThickness;

            var xArray = points.ConvertAll(p => points.First().Y - p.Y);
            var yArray = points.ConvertAll(p => railPoint.X - p.X);
            var index = 0;
            var yMax = Math.Abs(profile.StartPoint.Y - profile.EndPoint.Y);
            var allPenetration = PenetrationBegin.GetValueOrDefault() + (points.First().X < railPoint.X ? profileWidth : 0);
            //ProcessingObjectBuilder.AddEntity(profile);
            do
            {
                (var fn, index) = Helpers.FindMax(xArray, yArray, y - ToolThickness, y, index);
                // ProcessingObjectBuilder.AddEntity(new Circle(new Point3d(profile.StartPoint.X - x, profile.EndPoint.Y + y, 0), Vector3d.ZAxis, 1));
                // ProcessingObjectBuilder.AddEntity(new Circle(new Point3d(profile.StartPoint.X - (x-ToolThickness), profile.EndPoint.Y + y, 0), Vector3d.ZAxis, 1));
                var penetration = allPenetration - fn;
                var count = (int)Math.Ceiling(penetration / PenetrationStep.Value);
                var penetrationStepCalc = penetration / count;

                for (var i = count - 1; i >= 0; i--)
                {
                    var offset = fn + i * penetrationStepCalc;
                    var dz = profileHeight - y;
                    var toolpath = CreateCopy(rail, -offset + offsetVector.X, dz + offsetVector.Y);
                    ProcessingObjectBuilder.AddEntity(toolpath);
                    ProcessingObjectBuilder.AddEntity(NoDraw.Line(toolpath.StartPoint,
                        toolpath.StartPoint + (IsA90 ? Vector3d.ZAxis : Vector3d.YAxis) * ToolThickness));
                    ProcessingObjectBuilder.AddEntity(new Circle(toolpath.StartPoint, Vector3d.XAxis, 1));
                }
                //ProcessingObjectBuilder.AddEntity(NoDraw.Line(toolpath.StartPoint, toolpath.StartPoint + (IsA90 ? -Vector3d.YAxis : Vector3d.ZAxis) * 100));
                //if (Processor.IsUpperTool)
                //{
                //    var angleC = rail.GetToolAngle(rail.StartPoint, (Side)side);
                //    Processor.Move(tp[0].StartPoint, angleC, 90);
                //}

                //tp.ForEach(p => Cutting(p, fromStart = !fromStart, (Side)side));
                y += step;
            } while (y < yEnd);
        }


        //var xIndex = IsA90 ? 1 : 0;
        //if (profile.StartPoint[xIndex] < profile.EndPoint[xIndex])
        //    profile.ReverseCurve();

            //var side = (int)rail.GetFirstDerivative(0).GetAngleTo(Vector3d.XAxis).GetEngineSide() * ChangeEngineSide.GetSign(-1);
            //var isMinToolCoord = IsA90
            //    ? true
            //    : side == 1 ^ false ^ ChangeProcessSide;
         

        //Curve outletCurve = null;
        //if (IsA90 && IsOutlet)
        //{
        //    outletCurve = rail.GetOffsetCurves(TechProcess.ZSafety * processSide)[0] as Curve;
        //    var angleC = outletCurve.GetToolAngle(outletCurve.StartPoint, (Side)side);
        //    Processor.Move(outletCurve.StartPoint.X, outletCurve.StartPoint.Y, angleC: angleC, angleA: 90);
        //}
        //var angleA = IsA90 ? 90 : 0;
        //var sign = IsA90 ? -1 : 1;
        //var dist = profile.StartPoint[xIndex] - profile.EndPoint[xIndex];
        //var step = StepPass.Value;
        //var x = FirstPass.HasValue ? dist - FirstPass.Value : step;
        //var end = LastPass.HasValue ? dist - LastPass.Value : dist - ToolThickness;

        //var points = profile.GetPolylineFitPoints(1D).ToArray();

        //var xArray = points.ConvertAll(p => profile.StartPoint[xIndex] - p[xIndex]);
        //var yArray = points.ConvertAll(p => sign * (p[1 - xIndex] - profile.EndPoint[1 - xIndex]));
        //var index = 0;
        //var offsetVector = profile.EndPoint - profilePoint;
        //do
        //{
        //    (var y, index) = Helpers.FindMax(xArray, yArray, x - ToolThickness, x, index);

        //    //var allPenetration = PenetrationBegin.Value - end;
        //    //count = (int)Math.Ceiling(allPenetration / PenetrationStep.Value);
        //    //penetrationStepCalc = allPenetration / count;

        //    var (offset, dz) = IsA90 ? (y, dist - x) : (dist - x, y);
        //    var toolpath = CreateCopy(rail, sign * offset + offsetVector.X, dz + offsetVector.Y);
        //    // ProcessingObjectBuilder.AddEntity(toolpath);
        //    // ProcessingObjectBuilder.AddEntity(NoDraw.Line(toolpath.StartPoint, toolpath.StartPoint + Vector3d.ZAxis * 100));
        //    // ProcessingObjectBuilder.AddEntity(NoDraw.Line(toolpath.StartPoint, toolpath.StartPoint + Vector3d.YAxis * ToolThickness));
        //    //ProcessingObjectBuilder.AddEntity(new Circle(toolpath.StartPoint, Vector3d.XAxis, 3));
        //    //if (Processor.IsUpperTool)
        //    //{
        //    //    var angleC = rail.GetToolAngle(rail.StartPoint, (Side)side);
        //    //    Processor.Move(tp[0].StartPoint, angleC, 90);
        //    //}

        //    //tp.ForEach(p => Cutting(p, fromStart = !fromStart, (Side)side));
        //    x += step;
        //} 
        //while (x < end);

/*
        //var points = BuilderUtils.GetProcessPoints(profile, index, StepPass, ToolThickness, isMinToolCoord, FirstPass, LastPass, IsExactlyBegin, IsExactlyEnd, IsProfileStep);

            foreach (var point in points)
        {
            var end = Math.Max(point[1 - index], PenetrationEnd ?? double.MinValue);
            var count = 1;
            var penetrationStepCalc = 0D;
            if (DzBillet != 0)
            {
                count = (int)Math.Ceiling(DzBillet / PenetrationStep.Value);
                penetrationStepCalc = DzBillet / count;
            }
            else if (PenetrationStep.GetValueOrDefault() > 0 && PenetrationBegin.GetValueOrDefault() > end)
            {
                var allPenetration = PenetrationBegin.Value - end;
                count = (int)Math.Ceiling(allPenetration / PenetrationStep.Value);
                penetrationStepCalc = allPenetration / count;
            }
            //if (IsA90 && IsOutlet && Processor._isEngineStarted)
            //    Processor.Transition(z: point[index]);

            //var point = curve.GetPoint(fromStart);
            //if (processor.IsUpperTool)
            //{
            //    var angleC = curve.GetToolAngle(point, engineSide);
            //    processor.Move(point, angleC, angleA);
            //    //processor.Cycle();
            //}

            //processor.Penetration(point);

            var fromStart = true;

            var coords = Enumerable.Range(1, count).Select(p => end + (count - p) * penetrationStepCalc).ToList();
            var tp = coords.ConvertAll(p => CreateCopy(rail, -p, point[index]));
            if (Processor.IsUpperTool)
            {
                var angleC = rail.GetToolAngle(rail.StartPoint, (Side)side);
                Processor.Move(tp[0].StartPoint, angleC, 90);
            }

            tp.ForEach(p => Cutting(p, fromStart = !fromStart, (Side)side));
            //if (IsA90)
            //    coords.ForEach(p => Cutting(rail, processSide * p, point[index], fromStart = !fromStart, (Side)side));
            //else
            //    coords.ForEach(p => Cutting(rail, processSide * point[index], p, fromStart = !fromStart, (Side)side));

            //if (IsOutlet)
            //    if (IsA90)
            //    {
            //        var pt = outletCurve.GetClosestPoint(Processor.ToolPosition.Point);
            //        Processor.Move(pt.X, pt.Y);
            //    }
            //    else
        }
        rail.Dispose();
        Processor.Uplifting();
    */
    }

    public Curve CreateCopy(Curve curve, double offset = 0, double dz = 0)
    {
        Curve copy = null;
        if (offset != 0)
            copy = curve.GetOffsetCurves(offset)[0] as Curve;

        if (dz != 0)
        {
            var matrix = Matrix3d.Displacement(Vector3d.ZAxis * dz);
            if (copy == null)
                copy = curve.GetTransformedCopy(matrix) as Curve;
            else
                copy.TransformBy(matrix);
        }

        return copy;
    }

    public void Cutting(Curve curve, bool fromStart, Side engineSide)
    {
        var pt = curve.GetPoint(fromStart);
        Processor.Penetration(pt);
        switch (curve)
        {
            case Line line:
                Processor.GCommand(1, curve: line, point: curve.GetPoint(!fromStart));
                break;

            case Arc arc:
                var point = arc.GetPoint(fromStart);
                var angleC = arc.GetToolAngle(point, engineSide);
                var gCode = point == arc.StartPoint ? 3 : 2;
                Processor.GCommand(gCode, curve: arc, point: point, angleC: angleC, arcCenter: arc.Center.ToPoint2d());
                break;

            //case Polyline polyline:
            //    if (isExactlyBegin) polyline.SetPointAt(0, polyline.GetPointAtDist(indent).ToPoint2d());
            //    if (isExactlyEnd) polyline.SetPointAt(polyline.NumberOfVertices - 1, polyline.GetPointAtDist(polyline.Length - indent).ToPoint2d());
            //    Processor.Cutting(polyline, fromStart, feed, engineSide);
            //    break;

            default: throw new Exception();
        }
    }

    private void CreateProfile3D(Curve profile, Point3d profilePoint, Curve rail, Point3d railPoint)
    {
            var angle = Math.PI - rail.GetTangent(railPoint).MinusPiToPiAngleTo(Vector2d.YAxis);
            var matrix = Matrix3d.Displacement(railPoint - profilePoint) *
                Matrix3d.Rotation(angle, Vector3d.ZAxis, profilePoint) *
                Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, profilePoint);
            var p = profile.GetTransformedCopy(matrix);
            p.Transparency = Acad.GetSemitransparent();
            ProcessingObjectBuilder.AddEntity(p);
    }

    private Curve CreateDepartureRail(Curve curve, double departure)
    {
        if (curve is Line line)
        {
            var vector = line.Delta.GetNormal() * departure;
            return new Line(line.StartPoint - vector, line.EndPoint + vector);
        }
        var polyline = new Polyline();

        var startPoint = curve.StartPoint - curve.GetFirstDerivative(curve.StartPoint).GetNormal() * departure;
        polyline.AddVertexAt(0, startPoint.ToPoint2d(), 0, 0, 0);

        if (curve is Polyline poly)
            polyline.JoinPolyline(poly);
        else if (curve is Arc arc)
        {
            var bulge = Algorithms.GetArcBulge(arc, curve.StartPoint);
            polyline.AddVertexAt(1, curve.StartPoint.ToPoint2d(), bulge, 0, 0);
            polyline.AddVertexAt(2, curve.EndPoint.ToPoint2d(), 0, 0, 0);
        }
        else
            throw new Exception($"Тип кривой {curve.GetType().Name} не поддерживается");

        var endtPoint = curve.EndPoint + curve.GetFirstDerivative(curve.EndPoint).GetNormal() * departure;
        polyline.AddVertexAt(polyline.GetPolyPoints().Count(), endtPoint.ToPoint2d(), 0, 0, 0);

        return polyline;
    }
}