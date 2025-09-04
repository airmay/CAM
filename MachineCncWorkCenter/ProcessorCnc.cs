using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;

namespace CAM.CncWorkCenter
{
    public class ProcessorCnc : ProcessorBase
    {
        private readonly PostProcessorCnc _postProcessorCnc;
        private readonly ProcessingCnc _processingCnc;
        protected override ProcessingBase Processing => _processingCnc;
        protected override PostProcessorBase PostProcessor => _postProcessorCnc;

        public ProcessorCnc(ProcessingCnc processingCnc, PostProcessorCnc postProcessorCnc)
        {
            _processingCnc = processingCnc;
            _postProcessorCnc = postProcessorCnc;
        }

        public override void StartOperation(double zMax = 0)
        {
            base.StartOperation(zMax);
            AddCommands(_postProcessorCnc.SetTool(_processingCnc.Tool.Number, 0, 0, 0));
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

        public void Cutting(Point3d point) => GCommandTo(1, point, _processingCnc.CuttingFeed);

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
                AddCommands(_postProcessorCnc.StartEngine(_processingCnc.Frequency, true));
                IsEngineStarted = true;
            }
        }

        public void TurnC(double angleC) => GCommand(0, angleC: angleC);

        public void TurnA(double angleA) => GCommand(1, feed: 500, angleA: angleA);

        public void Uplifting() => GCommandTo(0, ToolPoint.WithZ(UpperZ));

        public void Penetration(Point3d point) => GCommandTo(1, point, _processingCnc.PenetrationFeed);

        public void GCommandTo(int gCode, Point3d point, int? feed = null)
        {
            if (point.IsEqualTo(ToolPoint))
                return;
            var line = NoDraw.Line(ToolPoint, point);

            GCommand(gCode, feed, line, point);
        }

        public void GCommand(int gCode, int? feed = null, Curve curve = null, Point3d? point = null, double? angleC = null, double? angleA = null, Point2d? arcCenter = null)
        {
            var commandText = _postProcessorCnc.GCommand(gCode, point, angleC?.ToRoundDeg(), angleA?.ToRoundDeg(), feed, arcCenter);
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

            AddCommand(commandText, point, angleC, angleA, duration, toolpath);
        }
    }
}