﻿using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Core;
using Dreambuild.AutoCAD;

namespace CAM.CncWorkCenter
{
    public class ProcessorCnc : IProcessor
    {
        private readonly PostProcessorCnc _postProcessor;
        private ToolpathBuilder _toolpathBuilder;
        private OperationCnc _operation;
        public bool IsEngineStarted;

        public ToolLocationCnc Location { get; set; } = new ToolLocationCnc();

        public Program Program { get; set; }
        public Tool Tool { get; set; }
        public int Frequency { get; set; }
        public int CuttingFeed { get; set; }
        public int PenetrationFeed { get; set; }
        public Point2d Origin { get; set; }
        public Side EngineSide { get; set; }

        public double ZSafety { get; set; } = 20;
        public double ZMax { get; set; } = 0;
        public double UpperZ => ZMax + ZSafety;

        public ProcessorCnc(PostProcessorCnc postProcessor)
        {
            _postProcessor = postProcessor;
        }

        public void Start()
        {
            Program.Reset();
            _toolpathBuilder = new ToolpathBuilder();

            Location.Z = ZMax + ZSafety * 3;
            //_postProcessor.GCommand(-1, Position, 0, 0, null);

            AddCommands(_postProcessor.StartMachine());
            AddCommands(_postProcessor.SetTool(Tool.Number, 0, 0, 0));
        }

        public void SetOperation(OperationCnc operation)
        {
            _operation = operation;
            _operation.FirstCommandIndex = Program.Count;
        }

        public void Finish()
        {
            AddCommands(_postProcessor.StopEngine());
            AddCommands(_postProcessor.StopMachine());
            _toolpathBuilder.Dispose();
        }

        public void Cycle() => AddCommand(_postProcessor.Cycle());

        public void AddCommand(string text, string name = null, ObjectId? toolpath = null)
        {
            if (text == null)
                return;

            Program.AddCommand(new Command
            {
                Name = name,
                Text = text,
                ToolLocation = Location,
                ObjectId = toolpath,
                Operation = _operation,
            });
        }

        private void AddCommands(string[] commands)
        {
            Array.ForEach(commands, p => AddCommand(p));
        }

        #region GCommands

        public void Cutting(Line line, CurveTip tip)
        {
            var point = line.GetPoint(tip);
            if (Location.Z > ZMax)
            {
                var angleC = BuilderUtils.CalcToolAngle(line.Angle, EngineSide);
                Move(point, angleC);
                //Cycle();
            }
            Penetration(point);
            Cutting(line, line.NextPoint(point));
        }

        public void Move(Point3d point, double? angleC = null, double? angleA = null)
        {
            GCommandTo(CommandNames.Fast, 0, point.WithZ(Location.Z));
            if (Location.Z > UpperZ)
                GCommandTo(CommandNames.InitialMove, 0, point.WithZ(UpperZ));
            if (angleC.HasValue)
                TurnC(angleC.Value);
            if (angleA.HasValue)
                TurnA(angleA.Value);

            if (!IsEngineStarted)
            {
                AddCommands(_postProcessor.StartEngine(Frequency, true));
                IsEngineStarted = true;
            }
        }

        public void TurnC(double angleC) => GCommand("Поворот", 0, angleC: angleC);

        public void TurnA(double angleA) => GCommand("Наклон", 1, feed: 500, angleA: angleA);

        public void Uplifting() => GCommandTo(CommandNames.Uplifting, 0, Location.Point.WithZ(UpperZ));

        public void Penetration(Point3d point) => GCommandTo(CommandNames.Penetration, 1, point, PenetrationFeed);

        public void Penetration(double z) => Penetration(Location.Point.WithZ(z));

        public void Cutting(Point3d point) => GCommandTo(CommandNames.Cutting, 1, point, CuttingFeed);

        private void Cutting(Line line, Point3d point) => GCommand(CommandNames.Cutting, 1, CuttingFeed, line, point);

        public void GCommandTo(string name, int gCode, Point3d point, int? feed = null)
        {
            Line line = null;
            if (Location.IsDefined)
            {
                if (point.IsEqualTo(Location.Point))
                    return;
                line = NoDraw.Line(Location.Point, point);
            }

            GCommand( name, gCode, feed, line, point);
        }

        public void GCommand(string name, int gCode, int? feed = null, Curve curve = null, Point3d? point = null,
            double? angleC = null, double? angleA = null, Point2d? arcCenter = null)
        {
            Location = Location.With(point, angleC, angleA);
            var commandText = _postProcessor.GCommand(gCode, Location, feed, arcCenter);
            ObjectId? toolpath = null;
            if (curve != null)
            {
                if (curve.IsNewObject)
                    _operation.Duration += curve.Length() / feed.GetValueOrDefault(10000) * 60;
                // todo проверить что после добавления curve.IsNewObject убрали
                if (curve.Length() > 1)
                    toolpath = _toolpathBuilder.AddToolpath(curve, name);
            }

            AddCommand(commandText, name, toolpath);
        }

        #endregion
    }
}