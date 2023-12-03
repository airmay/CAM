using System;
using CAM.Program.Generator;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Windows.ToolPalette;
using Dreambuild.AutoCAD;
using System.Data.Common;
using System.Drawing.Drawing2D;
using ColorControl;
using CAM;

namespace CAM
{
    public class Processor
    {
        private readonly IPostProcessor _postProcessor;
        private ToolpathBuilder _toolpathBuilder;
        public bool IsEngineStarted;
        public List<Command> ProcessCommands { get; } = new List<Command>();
        public Point3d ToolPosition { get; set; }
        public double AngleA { get; set; }
        public double AngleC { get; set; }

        private Operation _operation;
        protected int _frequency;
        public int CuttingFeed { get; set; }
        public int PenetrationFeed { get; set; }
        public double ZSafety { get; set; }
        public double UpperZ { get; set; }
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
            _frequency = generalOperation.Frequency;
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

        public void Finish() => _postProcessor.Finish();

        public void Cycle() => _postProcessor.Cycle();

        public void Uplifting() => GCommand(CommandNames.Uplifting, 0, z: UpperZ);

        public void Move(double? x = null, double? y = null, double? z = null, double? angleC = null, double? angleA = null)
        {
            GCommand(CommandNames.Fast, 0, x: x, y: y, z: UpperZ + ZSafety * 3);
            GCommand(CommandNames.Fast, 0, angleC: angleC);
            if (!IsEngineStarted)
                GCommand(CommandNames.InitialMove, 0, z: UpperZ);

            if (angleA.HasValue && angleA.Value != AngleA)
                GCommand("Наклон", 1, angleA: angleA, feed: 500);

            if (!IsEngineStarted)
            {
                _postProcessor.StartEngine(_frequency, true);
                IsEngineStarted = true;
            }
        }

        public void Penetration(Point3d point)
        {
            GCommand(CommandNames.Penetration, 1, point: point, feed: PenetrationFeed);
        }

        //public void Cutting(Line line, Point3d point) => GCommand(CommandNames.Cutting, 1, point: point, curve: line, feed: CuttingFeed);
        //public void Cutting(Arc arc, Point3d point, Point3d center) => GCommand(CommandNames.Cutting, gCode, point: curve.NextPoint(point), angleC: angleC, curve: curve, feed: cuttingFeed, center: arc?.Center.ToPoint2d());


        //{
        //if (curve is Polyline polyline)
        //{
        //    if (point == polyline.EndPoint)
        //    {
        //        polyline.ReverseCurve();
        //        engineSide = engineSide.Opposite();
        //    }
        //    for (int i = 1; i < polyline.NumberOfVertices; i++)
        //    {
        //        point = polyline.GetPoint3dAt(i);
        //        if (calcAngleC)
        //            angleC = BuilderUtils.CalcToolAngle(polyline, point, engineSide);
        //        if (polyline.GetSegmentType(i - 1) == SegmentType.Arc)
        //        {
        //            var arcSeg = polyline.GetArcSegment2dAt(i - 1);
        //            GCommand(CommandNames.Cutting, arcSeg.IsClockWise ? 2 : 3, point: point, angleC: angleC, curve: polyline, feed: cuttingFeed, center: arcSeg.Center);
        //        }
        //        else
        //            GCommand(CommandNames.Cutting, 1, point: point, angleC: angleC, curve: polyline, feed: cuttingFeed);
        //    }
        //}
        //else
        //{
        //    var arc = curve as Arc;
        //    if (arc != null && calcAngleC)
        //        angleC += arc.TotalAngle.ToDeg() * (point == curve.StartPoint ? -1 : 1);
        //    var gCode = curve is Line ? 1 : point == curve.StartPoint ? 3 : 2;
        //    GCommand(CommandNames.Cutting, gCode, point: curve.NextPoint(point), angleC: angleC, curve: curve, feed: cuttingFeed, center: arc?.Center.ToPoint2d());
        //}
        //}

        //    public void GCommand(string name, int gCode, Point3d? point = null, double? x = null, double? y = null, double? z = null,
        //double? angleC = null, double? angleA = null, Curve curve = null, int? feed = null, Point2d? center = null)
        //    {
        //        var command = new Command { Name = name };

        //        var position = ToolPosition.Create(point, x, y, z, angleC, angleA);

        //        if (ToolPosition.IsDefined)
        //        {
        //            double length;
        //            if (curve == null)
        //            {
        //                length = ToolPosition.Point.DistanceTo(position.Point);
        //                if (length > 1)
        //                    curve = NoDraw.Line(ToolPosition.Point, position.Point);
        //            }
        //            else
        //                length = curve.Length();

        //            if (curve != null && curve.IsNewObject)
        //            {
        //                if (Colors.ContainsKey(name))
        //                    curve.Color = Colors[name];
        //                curve.LayerId = _layerId;
        //                curve.Visible = false;
        //                _currentSpace.AppendEntity(curve);
        //                _transaction.AddNewlyCreatedDBObject(curve, true);
        //            }
        //            command.ToolpathObjectId = curve?.ObjectId;

        //            if ((feed ?? _feed) != 0)
        //                command.Duration = length / (gCode == 0 ? 10000 : feed ?? _feed) * 60;
        //        }

        //        if (position.IsDefined)
        //            command.ToolLocation = position;

        //        ToolPosition = position;
        //        command.Text = _postProcessor.GCommand(gCode, position, feed, center);
        //        command.HasTool = _hasTool;
        //        AddCommand(command);

        //        _feed = feed ?? _feed;
        //    }

        public void GCommand(string name, int gCode, Point3d point, int? feed = null)
        {

        }

        public void Cutting(Line line, Point3d point)
        {
            var command = new Command
            {
                Name = CommandNames.Cutting,
                Point = point,
                AngleA = AngleA,
                AngleC = AngleC
            };
            command.Toolpath = _toolpathBuilder.AddToolpath(line, command.Name);

            var length = line.Length();
            _operation.Duration += length / CuttingFeed * 60;

            command.Text = _postProcessor.GCommand(1, point, AngleC, AngleA, CuttingFeed);
            AddCommand(command);
        }

        public void AddCommand(Command command)
        {
            ProcessCommands.Add(command);
            command.Operation = _operation;
            command.Number = ProcessCommands.Count;

            if (_operation.FirstCommandIndex == 0)
                _operation.FirstCommandIndex = ProcessCommands.Count - 1;
        }

    }
}