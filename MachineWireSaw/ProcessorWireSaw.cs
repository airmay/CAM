using Autodesk.AutoCAD.Geometry;
using CAM.CncWorkCenter;
using CAM.Core;
using Dreambuild.AutoCAD;
using System;

namespace CAM.MachineWireSaw
{
    public class ProcessorWireSaw : IProcessor
    {
        private readonly PostProcessorWireSaw _postProcessor;
        private readonly ProcessingWireSaw _processing;
        private IOperation _operation;
        public bool IsEngineStarted;
        private double _processDuration;
        private double _operationDuration;

        public ProcessorWireSaw(ProcessingWireSaw processing, PostProcessorWireSaw postProcessor)
        {
            _processing = processing;
            _postProcessor = postProcessor;
        }

        public void SetOperation(IOperation operation)
        {
            if (_operation != null)
                FinishOperation();
            _operation = operation;
        }

        private void FinishOperation()
        {
            _operation.Caption = GetCaption(_operation.Caption, _operationDuration);
            _processDuration += _operationDuration;
            _operationDuration = 0;
        }

        private static string GetCaption(string caption, double duration)
        {
            var ind = caption.IndexOf('(');
            var timeSpan = new TimeSpan(0, 0, 0, (int)duration);
            return $"{(ind > 0 ? caption.Substring(0, ind).Trim() : caption)} ({timeSpan})";
        }

        public void Dispose() { }

        public void Start()
        {
            ProcessingBase.Program.Init(_processing);
        }

        public void Finish()
        {
            AddCommands(_postProcessor.StopEngine());
            AddCommands(_postProcessor.StopMachine());
            ProcessingBase.Program.CreateProgram();
            FinishOperation();
            _processing.Caption = GetCaption(_processing.Caption, _processDuration);
        }

        public void StartOperation(double angle, double u, double v)
        {
            if (IsEngineStarted)
                return;

            //IsEngineStarted = true;
            //Angle = angle;
            //U = u;
            //V = v;

            AddCommands(_postProcessor.StartMachine());
        }

        public void Pause(double duration)
        {
            AddCommand("(DLY,{duration})", "Пауза");
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
        public double Angle { get; set; } = 90;
        public double U { get; set; }
        public double V { get; set; }

        public int Feed => _processing.CuttingFeed;
        public Point2d Center => _processing.Origin.Point;
        private Vector2d _uVector = -Vector2d.XAxis;
        private Point2d _toolPoint;

        public void AddCommand(string text, string name = null)
        {
            if (text == null)
                return;

            var toolLocationParams = new ToolLocationParams(_toolPoint.X, _toolPoint.Y, V, Angle, 0);

            ProcessingBase.Program.AddCommand(new Command
            {
                Name = name,
                Text = text,
                ToolLocationParams = toolLocationParams,
                Operation = _operation,
            });
        }

        private void AddCommands(string[] commands) => Array.ForEach(commands, p => AddCommand(p));
        
        public void Move(Point2d point1, Point2d point2, bool isReverseAngle, bool isReverseU)
        {
            GCommands(0, point1, point2, null, isReverseAngle, isReverseU);

            if (!IsEngineStarted)
            {
                AddCommands(_postProcessor.StartEngine());
                IsEngineStarted = true;
            }
        }

        public void Cutting(Line3d line) => Cutting(line.StartPoint, line.EndPoint);

        public void Cutting(Point3d point1, Point3d point2) => Cutting(point1.To2d(), point2.To2d(), (point1.Z + point2.Z) / 2);

        public void Cutting(Point2d point1, Point2d point2, double z)
        {
            GCommands(1, point1, point2, z, false, false);
        }

        public void GCommands(int gCode, Point2d point1, Point2d point2, double? z, bool isReverseAngle, bool isReverseU)
        {
            var line = new Line2d(point1, point2);
            Angle = line.Direction.Angle.ToRoundDeg() % 180;
            _toolPoint = line.GetClosestPointTo(Center).Point;
            var vector = _toolPoint - Center;
            var u = vector.Length.Round(3);
            if (u == 0)
                vector = line.Direction.GetPerpendicularVector();
            var uSign = vector.DotProduct(_uVector).GetSign();
            uSign *= -isReverseU.GetSign();

            GCommandA(vector * uSign, isReverseAngle);
            GCommandUV(gCode, u * uSign, (z ?? V).Round(3));
        }

        private void GCommandA(Vector2d vector, bool isReverseAngle)
        {
            var da = _uVector.MinusPiToPiAngleTo(vector).ToRoundDeg();
            if (da == 0) 
                return;

            if (isReverseAngle)
                da -= 360 * da.GetSign();
            AddCommand($"G05 A{da} S{_processing.S}", "Rotate");

            _uVector = _uVector.RotateBy(da.ToRad());
            _operationDuration += Math.Abs(da) / _processing.S * 60;
        }

        private void GCommandUV(int gCode, double u, double v)
        {
            var du = u - U;
            var dv = v - V;
            if (du == 0 && dv == 0)
                return;

            var text = $"G0{gCode} U{du} V{dv}";
            if (gCode == 1)
                text += $" F{Feed}";
            AddCommand(text);
            U = u;
            V = v;
            _operationDuration += Math.Sqrt(du * du + dv * dv) / (gCode == 0 ? 500 : Feed) * 60;
        }
    }
}