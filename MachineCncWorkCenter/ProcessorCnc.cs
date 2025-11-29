using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Autocad;
using CAM.Autocad.AutoCADCommands;
using CAM.Core.Enums;
using CAM.Core.Processing;
using CAM.Core.Tools;
using CAM.MachineCncWorkCenter.PostProcessors;
using CAM.Utils;
using System;

namespace CAM.MachineCncWorkCenter;

public class ProcessorCnc : ProcessorBase<TechProcessCnc, ProcessorCnc>
{
    private PostProcessorCnc _postProcessor;
    protected override PostProcessorBase PostProcessor => _postProcessor;

    protected override void CreatePostProcessor()
    {
        _postProcessor = TechProcess.Machine switch
        {
            Machine.Donatoni => new DonatoniPostProcessor(),
            Machine.Krea => new KreaPostProcessor(),
            Machine.Forma => new FormaPostProcessor(),
            Machine.Champion => new ChampionPostProcessor(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override void StartOperation(double? zMax = null)
    {
        base.StartOperation(zMax);
        var tool = Operation.GetTool();
        if (tool != Tool)
        {
            AddCommands(_postProcessor.SetTool(tool.Number, 0, 0, 0));
            Tool = tool;
        }
    }

    public void Cycle() => AddCommand(_postProcessor.Cycle());

    public void Cutting(Point3d startPoint, Point3d endPoint)
    {
        if (IsUpperTool)
        {
            var angleC = (endPoint - startPoint).ToVector2d().Angle.GetToolAngle();
            Move(startPoint, angleC);
        }

        Penetration(startPoint);
        Cutting(endPoint);
    }

    public void Cutting(Point3d point) => GCommandTo(1, point, TechProcess.CuttingFeed);

    /// <summary>
    /// Быстрое перемещение по верху к точке над заданной
    /// </summary>
    public void Move(Point3d point, double? angleC = null, double? angleA = null)
    {
        if (!IsUpperTool)
            Uplifting();

        GCommandTo(0, point.WithZ(ToolPoint.Z));
        if (ToolPoint.Z - UpperZ > 1)
            GCommandTo(0, point.WithZ(UpperZ));
        if (angleC.HasValue)
            TurnC(angleC.Value);
        if (angleA.HasValue && !angleA.Value.IsEqual(AngleA))
            TurnA(angleA.Value);

        if (!IsEngineStarted)
        {
            AddCommands(_postProcessor.StartEngine(TechProcess.Frequency, true));
            IsEngineStarted = true;
        }
    }

    protected override void MoveToPosition(ToolPosition position)
    {
        Move(position.Point, position.AngleC, position.AngleA);
        Penetration(position.Point);
    }

    public void TurnC(double angleC) => GCommand(0, angleC: angleC);

    public void TurnA(double angleA) => GCommand(1, feed: 500, angleA: angleA);

    public void Uplifting() => GCommandTo(0, ToolPoint.WithZ(UpperZ));

    public void Penetration(Point3d point) => GCommandTo(1, point, TechProcess.PenetrationFeed);

    public void GCommandTo(int gCode, Point3d point, int? feed = null)
    {
        if (point.IsEqualTo(ToolPoint))
            return;

        var line = NoDraw.Line(ToolPoint, point);
        GCommand(gCode, feed, line, point);
    }

    public void GCommand(int gCode, int? feed = null, Curve curve = null, Point3d? point = null, double? angleC = null, double? angleA = null, Point2d? arcCenter = null)
    {
        var commandText = _postProcessor.GCommand(point, angleC?.ToRoundDeg(), angleA, feed, arcCenter);
        if (string.IsNullOrEmpty(commandText))
            return;

        ObjectId? toolpath = null;
        double? duration = null;
        if (curve != null)
        {
            if (curve.IsNewObject)
                duration = curve.Length() / (feed ?? 10000) * 60;

            toolpath = ProcessingObjectBuilder.AddToolpath(curve, gCode);
        }

        AddCommand($"G{gCode} {commandText}", point, angleC, angleA, duration, toolpath);
    }

    #region Cutting

    public void Cutting(Curve curve, int engineSide, Point3d? pt = null, double? angleC = null, double? angleA = null)
    {
        var point = pt ?? GetClosestToolPoint(curve);
        if (IsUpperTool)
            Move(point, angleC ?? curve.GetToolAngle(point, (Side)engineSide), angleA);
        Penetration(point);
        if (angleA.HasValue && angleA != AngleA)
            TurnA(angleA.Value);
        point = curve.NextPoint(point);

        switch (curve)
        {
            case Line line:
                Cutting(line, point);
                return;

            case Arc arc:
                Cutting(arc, point, engineSide);
                return;

            case Polyline polyline:
                Cutting(polyline, point, engineSide);
                return;

            default: throw new Exception();
        }
    }

    public void Cutting(Line line, Point3d point)
    {
        GCommand(1, curve: line, point: point, feed: TechProcess.CuttingFeed);
    }

    public void Cutting(Arc arc, Point3d point, int engineSide)
    {
        var gCode = point == arc.StartPoint ? 3 : 2;
        var angleC = arc.GetToolAngle(point, (Side)engineSide);
        GCommand(gCode, curve: arc, point: point, angleC: angleC, arcCenter: arc.Center.ToPoint2d(), feed: TechProcess.CuttingFeed);
    }

    public void Cutting(Polyline polyline, Point3d point, int engineSide)
    {
        if (polyline.IsStartPoint(point))
        {
            polyline.ReverseCurve();
            engineSide = -engineSide;
        }
        for (var i = 0; i < polyline.NumberOfVertices - 1; i++)
        {
            point = polyline.GetPoint3dAt(i + 1);
            var angleC = polyline.GetToolAngle(point, (Side)engineSide);
            var gCode = 1;
            Point2d? center = null;
            if (polyline.GetSegmentType(i) == SegmentType.Arc)
            {
                var arcSeg = polyline.GetArcSegment2dAt(i);
                gCode = arcSeg.IsClockWise ? 2 : 3;
                center = arcSeg.Center;
            }
            GCommand(gCode, TechProcess.CuttingFeed, polyline, point, angleC: angleC, arcCenter: center);
        }
    }
    #endregion
}