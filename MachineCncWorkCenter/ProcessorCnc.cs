using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;

namespace CAM.CncWorkCenter
{
    public class ProcessorCnc : ProcessorBase<ProcessingCnc, ProcessorCnc>
    {
        private PostProcessorCnc _postProcessor;
        protected override PostProcessorBase PostProcessor => _postProcessor;

        public override void Start()
        {
            _postProcessor = Processing.GetPostProcessor();
            base.Start();
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

        public void Cutting(Point3d startPoint, Point3d endPoint)
        {
            if (IsUpperTool)
            {
                var angleC = BuilderUtils.CalcToolAngle((endPoint - startPoint).ToVector2d().Angle);
                Move(startPoint, angleC);
            }

            Penetration(startPoint);
            Cutting(endPoint);
        }

        public void Cutting(Point3d point) => GCommandTo(1, point, Processing.CuttingFeed);

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
                AddCommands(_postProcessor.StartEngine(Processing.Frequency, true));
                IsEngineStarted = true;
            }
        }

        public void TurnC(double angleC) => GCommand(0, angleC: angleC);

        public void TurnA(double angleA) => GCommand(1, feed: 500, angleA: angleA);

        public void Uplifting() => GCommandTo(0, ToolPoint.WithZ(UpperZ));

        public void Penetration(Point3d point) => GCommandTo(1, point, Processing.PenetrationFeed);

        public void GCommandTo(int gCode, Point3d point, int? feed = null)
        {
            if (point.IsEqualTo(ToolPoint))
                return;
            var line = NoDraw.Line(ToolPoint, point);

            GCommand(gCode, feed, line, point);
        }

        public void GCommand(int gCode, int? feed = null, Curve curve = null, Point3d? point = null, double? angleC = null, double? angleA = null, Point2d? arcCenter = null)
        {
            var commandText = _postProcessor.GCommand(gCode, point, angleC?.ToRoundDeg(), angleA?.ToRoundDeg(), feed, arcCenter);
            if (commandText == null)
                return;

            ObjectId? toolpath = null;
            double? duration = null;
            if (curve != null)
            {
                if (curve.IsNewObject)
                    duration = curve.Length() / (feed ?? 10000) * 60;

                toolpath = ToolpathBuilder.AddToolpath(curve, gCode);
            }

            AddCommand(commandText, point, -angleC, angleA, duration, toolpath);
        }
    }
}