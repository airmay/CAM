using System;
using CAM.Program.Generator;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Windows.ToolPalette;
using Dreambuild.AutoCAD;

namespace CAM
{
    public class Processor
    {
        private readonly IPostProcessor _postProcessor;
        private ToolpathBuilder _toolpathBuilder;
        public List<Command> ProcessCommands { get; } = new List<Command>();
        public Point3d ToolPosition { get; set; }
        public double AngleA { get; set; }
        public double AngleC { get; set; }

        private Operation _operation;
        public int CuttingFeed { get; set; }
        public int PenetrationFeed { get; set; }
        public double ZSafety { get; set; }
        public double ZMax { get; set; }
        public double OriginX { get; set; }
        public double OriginY { get; set; }

        public Processor(IPostProcessor postProcessor)
        {
            _postProcessor = postProcessor;
        }

        public void Start(Tool tool)
        {
            _postProcessor.StartMachine();
            _postProcessor.SetTool(tool.Number, 0, 0, 0);
            _toolpathBuilder = new ToolpathBuilder();
        }

        public void SetGeneralOperarion(GeneralOperation generalOperation)
        {
            CuttingFeed = generalOperation.CuttingFeed;
            PenetrationFeed = generalOperation.PenetrationFeed;
            ZSafety = generalOperation.ZSafety;
            OriginX = generalOperation.OriginX;
            OriginY = generalOperation.OriginY;
        }

        public void SetOperarion(Operation operation)
        {
            _operation = operation;
            _operation.FirstCommandIndex = ProcessCommands.Count;
        }

        public void Finish()
        {
        }

        public void Cutting(Curve curve)
        {
            var point = ToolPosition.IsDefined ? curve.GetClosestPoint(ToolPosition.Point) : curve.StartPoint;
            var calcAngleC = angleC == null;

            if (IsUpperTool) // && (angleA ?? AngleA).GetValueOrDefault() == 0)
            {
                if (IsChangeStart)
                    point = curve.NextPoint(point);
                //var upperPoint = new Point3d(point.X, point.Y, ZSafety);
                //if (!ToolPosition.Point.IsEqualTo(upperPoint))
                //{
                if (!angleC.HasValue)
                    angleC = BuilderUtils.CalcToolAngle(curve, point, engineSide);

                Move(point.X, point.Y, angleC: angleC, angleA: angleA);
                //}
            }
            else if (!(curve is Line) && calcAngleC)
                angleC = BuilderUtils.CalcToolAngle(curve, point, engineSide);

            if (!point.IsEqual(ToolPosition.Point))
                GCommand(point.Z != ToolPosition.Point.Z ? CommandNames.Penetration : CommandNames.Transition, 1, point: point, angleC: angleC, angleA: angleA, feed: smallFeed);

            if (curve is Polyline polyline)
            {
                if (point == polyline.EndPoint)
                {
                    polyline.ReverseCurve();
                    engineSide = engineSide.Opposite();
                }
                for (int i = 1; i < polyline.NumberOfVertices; i++)
                {
                    point = polyline.GetPoint3dAt(i);
                    if (calcAngleC)
                        angleC = BuilderUtils.CalcToolAngle(polyline, point, engineSide);
                    if (polyline.GetSegmentType(i - 1) == SegmentType.Arc)
                    {
                        var arcSeg = polyline.GetArcSegment2dAt(i - 1);
                        GCommand(CommandNames.Cutting, arcSeg.IsClockWise ? 2 : 3, point: point, angleC: angleC, curve: polyline, feed: cuttingFeed, center: arcSeg.Center);
                    }
                    else
                        GCommand(CommandNames.Cutting, 1, point: point, angleC: angleC, curve: polyline, feed: cuttingFeed);
                }
            }
            else
            {
                var arc = curve as Arc;
                if (arc != null && calcAngleC)
                    angleC += arc.TotalAngle.ToDeg() * (point == curve.StartPoint ? -1 : 1);
                var gCode = curve is Line ? 1 : point == curve.StartPoint ? 3 : 2;
                GCommand(CommandNames.Cutting, gCode, point: curve.NextPoint(point), angleC: angleC, curve: curve, feed: cuttingFeed, center: arc?.Center.ToPoint2d());
            }
        }
    }
}