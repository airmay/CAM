using Autodesk.AutoCAD.DatabaseServices;
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
        private ToolpathBuilder _toolpathBuilder;
        private double _processDuration;
        private double _operationDuration;

        public ProcessorWireSaw(ProcessingWireSaw processing, PostProcessorWireSaw postProcessor)
        {
            _processing = processing;
            _postProcessor = postProcessor;

            _uAxis = -Vector2d.XAxis;
            _toolAngle = Math.PI / 2;
            U = 0;
        }

        public void SetOperation(IOperation operation)
        {
            if (_operation != null)
                FinishOperation();
            _operation = operation;
            _toolpathBuilder.CreateGroup();
        }

        private void FinishOperation()
        {
            _operation.ToolpathGroupId = _toolpathBuilder.AddGroup(_operation.Caption);
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

        public void Dispose()
        {
            _toolpathBuilder.Dispose();
        }

        public void Start()
        {
            Program.Init(_processing);
            _toolpathBuilder = new ToolpathBuilder();
        }

        public void Finish()
        {
            AddCommands(_postProcessor.StopEngine());
            AddCommands(_postProcessor.StopMachine());

            Program.CreateProgram();
            FinishOperation();
            _processing.Caption = GetCaption(_processing.Caption, _processDuration);
        }

        public bool IsEngineStarted;
        public double UpperZ;

        public void StartOperation(double zMax)
        {
            if (IsEngineStarted)
                return;

            V = UpperZ = zMax + _processing.ZSafety;
            _toolPoint = Center.WithZ(UpperZ);

            AddCommands(_postProcessor.StartMachine());
        }

        public void Pause(double duration)
        {
            AddCommand(_postProcessor.Pause(duration));
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
        private Vector2d _uAxis;
        private Point3d _toolPoint;
        private double _toolAngle;

        public double U { get; set; }
        public double V { get; set; }

        public int Feed => _processing.CuttingFeed;
        private Point2d Center => _processing.Origin.Point;

        public void AddCommand(string text, double? duration = null, ObjectId? toolpath1 = null, ObjectId? toolpath2 = null)
        {
            var command = new Command
            {
                Text = text,
                ToolPosition = new ToolPosition(_toolPoint, -_toolAngle),
                ObjectId = toolpath1,
                ObjectId2 = toolpath2,
                Operation = _operation,
            };
            if (duration.HasValue)
                command.Duration = new TimeSpan(0, 0, 0, (int)Math.Round(duration.Value)).ToString();

            Program.AddCommand(command);
            _operationDuration += duration.GetValueOrDefault();
        }

        private void AddCommands(string[] commands) => Array.ForEach(commands, p => AddCommand(p));

        #region public

        public void Move(Point3d point, Vector3d direction, bool isReverseAngle = false, bool isReverseU = false)
        {
            Move(point, point + direction, isReverseAngle, isReverseU);
        }

        public void Move(Point3d point1, Point3d point2, bool isReverseAngle = false, bool isReverseU = false)
        {
            if (UpperZ - V > 0.1)
                return;

            var z = (point1.Z + point2.Z) / 2;
            var line = new Line2d(point1.ToPoint2d(), point2.ToPoint2d());

            if (UpperZ - z > 0.001)
            {
                GCommands(0, line, UpperZ, isReverseAngle, isReverseU);
                isReverseAngle = false;
                isReverseU = false;
            }

            GCommands(0, line, z, isReverseAngle, isReverseU);
        }

        public void Cutting(Point3d point, Vector3d direction) => Cutting(point, point + direction);
        
        public void Cutting(Line3d line) => Cutting(line.StartPoint, line.EndPoint);

        public void Cutting(Point3d point1, Point3d point2)
        {
            if (!IsEngineStarted)
            {
                AddCommands(_postProcessor.StartEngine());
                IsEngineStarted = true;
            }
            GCommands(1, point1, point2, false, false);
        }

        #endregion

        private void GCommands(int gCode, Point3d point1, Point3d point2, bool isReverseAngle, bool isReverseU)
        {
            GCommands(gCode, new Line2d(point1.ToPoint2d(), point2.ToPoint2d()), (point1.Z + point2.Z) / 2, isReverseAngle, isReverseU);
        }

        private void GCommands(int gCode, Line2d line, double z, bool isReverseAngle, bool isReverseU)
        {
            var point = line.GetClosestPointTo(Center).Point;
            var centerVector = point - Center;
            var centerDist = centerVector.Length.Round(3);
            if (centerDist == 0)
                centerVector = line.Direction.GetPerpendicularVector();
            var uSign = centerVector.DotProduct(_uAxis).GetSign();
            uSign *= isReverseU.GetSign(-1);

            GCommandA(centerVector * uSign, isReverseAngle, line.Direction.Angle);

            GCommandUV(gCode, centerDist * uSign, z, point);
        }

        private void GCommandA(Vector2d vector, bool isReverseAngle, double newToolAngle)
        {
            // + это поворот стола По часовой
            var da = _uAxis.MinusPiToPiAngleTo(vector).ToRoundDeg();
            if (da == 0) 
                return;

            if (isReverseAngle)
                da -= 360 * da.GetSign();

            var daRad = da.ToRad();
            var toolVector = Vector3d.XAxis.RotateBy(_toolAngle, Vector3d.ZAxis) * ToolObject.WireSawLength;
            _toolAngle = newToolAngle;

            var duration = Math.Abs(da) / _processing.S * 60;
            AddCommand($"G05 A{da} S{_processing.S}", duration, CreateToolpath(toolVector), CreateToolpath(-toolVector));

            _uAxis = _uAxis.RotateBy(daRad);

            return;

            ObjectId? CreateToolpath(Vector3d vector1)
            {
                return _toolpathBuilder.AddToolpath(NoDraw.ArcSCA(_toolPoint + vector1, Center.WithZ(_toolPoint.Z), daRad));
            }
        }

        private void GCommandUV(int gCode, double u, double v, Point2d point)
        {
            var du = (u - U).Round(3);
            var dv = (v - V).Round(3);
            if (du == 0 && dv == 0)
                return;

            U += du;
            V += dv;

            var commandText = $"G0{gCode} U{du} V{dv}";
            if (gCode == 1)
                commandText += $" F{Feed}";

            var toolVector = Vector3d.XAxis.RotateBy(_toolAngle, Vector3d.ZAxis) * ToolObject.WireSawLength;
            var newToolPoint = point.WithZ(v);
            var toolpath1 = CreateToolpath(toolVector);
            var toolpath2 = CreateToolpath(-toolVector);
            _toolPoint = newToolPoint;

            var duration = Math.Sqrt(du * du + dv * dv) / (gCode == 0 ? 500 : Feed) * 60;
            AddCommand(commandText, duration, toolpath1, toolpath2);
            
            return;

            ObjectId? CreateToolpath(Vector3d vector)
            {
                return _toolpathBuilder.AddToolpath(NoDraw.Line(_toolPoint + vector, newToolPoint + vector));
            }
        }
    }
}