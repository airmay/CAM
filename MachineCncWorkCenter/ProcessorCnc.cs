using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;

namespace CAM.CncWorkCenter
{
    public class ProcessorCnc : ProcessorBase
    {
        public Side EngineSide { get; set; }

        public ProcessorCnc(ProcessingCnc processing, PostProcessorCnc postProcessor)
        {
            _processing = processing;
            _postProcessor = postProcessor;
        }

        #region GCommands

        public void Cutting(Line line, CurveTip tip, int? feed = null)
        {
            var point = line.GetPoint(tip);
            if (IsUpperTool)
            {
                var angleC = BuilderUtils.CalcToolAngle(line.Angle, EngineSide);
                Move(point, angleC);
                //Cycle();
            }
            Penetration(point);
            Cutting(line, line.NextPoint(point), feed);
        }

        public void Move(Point3d point, double? angleC = null, double? angleA = null)
        {
            GCommandTo(CommandNames.Fast, 0, point.WithZ(ToolPoint.Z));
            if (IsUpperTool)
                GCommandTo(CommandNames.InitialMove, 0, point.WithZ(UpperZ));
            if (angleC.HasValue)
                TurnC(angleC.Value);
            if (angleA.HasValue && !angleA.Value.IsEqual(AngleA))
                TurnA(angleA.Value);

            if (!IsEngineStarted)
            {
                AddCommands(_postProcessor.StartEngine(_processing.Frequency, true));
                IsEngineStarted = true;
            }
        }

        public void TurnC(double angleC) => GCommand("Поворот", 0, angleC: angleC);

        public void TurnA(double angleA) => GCommand("Наклон", 1, feed: 500, angleA: angleA);

        public void Uplifting() => GCommandTo(CommandNames.Uplifting, 0, ToolPoint.WithZ(UpperZ));

        public void Penetration(Point3d point) => GCommandTo(CommandNames.Penetration, 1, point, _processing.PenetrationFeed);

        public void Penetration(double z) => Penetration(ToolPoint.WithZ(z));

        //public void Cutting(Point3d point) => GCommandTo(CommandNames.Cutting, 1, point, CuttingFeed);

        private void Cutting(Line line, Point3d point, int? feed = null) => GCommand(CommandNames.Cutting, 1, feed, line, point);

        public void GCommandTo(string name, int gCode, Point3d point, int? feed = null)
        {
            if (point.IsEqualTo(ToolPoint))
                return;
            var line = NoDraw.Line(ToolPoint, point);

            GCommand(name, gCode, feed, line, point);
        }

        public void GCommand(string name, int gCode, int? feed = null, Curve curve = null, Point3d? point = null,
            double? angleC = null, double? angleA = null, Point2d? arcCenter = null)
        {
            ToolPoint = point ?? ToolPoint;
            AngleC = angleC ?? AngleC;
            AngleA = angleA ?? AngleA;

            var commandText = _postProcessor.GCommand(gCode, ToolPoint, AngleC, AngleA, feed, arcCenter);
            ObjectId? toolpath = null;
            double? duration = null;
            if (curve != null)
            {
                if (curve.IsNewObject)
                    duration = curve.Length() / (feed ?? 10000) * 60;

                if (curve.Length() > 1)
                    toolpath = _toolpathBuilder.AddToolpath(curve, name);
            }

            AddCommand(commandText, duration, toolpath);
        }

        #endregion
    }
}