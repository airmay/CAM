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
        public bool IsEngineStarted;
        private double _processDuration;
        private double _operationDuration;
        public double UpperZ;
        private ToolpathBuilder _toolpathBuilder;

        public ProcessorWireSaw(ProcessingWireSaw processing, PostProcessorWireSaw postProcessor)
        {
            _processing = processing;
            _postProcessor = postProcessor;


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
            ProcessingBase.Program.Init(_processing);
            _toolpathBuilder = new ToolpathBuilder();
        }

        public void Finish()
        {
            AddCommands(_postProcessor.StopEngine());
            AddCommands(_postProcessor.StopMachine());
            ProcessingBase.Program.CreateProgram();
            FinishOperation();
            _processing.Caption = GetCaption(_processing.Caption, _processDuration);
        }

        public void StartOperation(double upperZ)
        {
            if (IsEngineStarted)
                return;

            U = 0;
            V = UpperZ = upperZ;
            _toolPoint = Center.WithZ(upperZ);

            AddCommands(_postProcessor.StartMachine());
        }

        public void Pause(double duration)
        {
            AddCommand("(DLY,{duration})", "Пауза");
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
        private const int ToolLength = 1500;
        private Point3d _toolPoint;
        private double _toolAngle;

        public double U { get; set; }
        public double V { get; set; }

        public int Feed => _processing.CuttingFeed;
        public Point2d Center => _processing.Origin.Point;
        private Vector2d _uAxis = -Vector2d.XAxis;

        public void AddCommand(string text, string name = null, ObjectId? toolpath1 = null, ObjectId? toolpath2 = null)
        {
            if (text == null)
                return;

            var toolLocationParams = new ToolLocationParams(_toolPoint.X, _toolPoint.Y, _toolPoint.Z, _toolAngle, 0);

            ProcessingBase.Program.AddCommand(new Command
            {
                Name = name,
                Text = text,
                ToolLocationParams = toolLocationParams,
                Operation = _operation,
                ObjectId = toolpath1,
                ObjectId2 = toolpath2,
            });
        }

        private void AddCommands(string[] commands) => Array.ForEach(commands, p => AddCommand(p));

        public void Move(Point3d point, Vector3d direction, bool isReverseAngle = false, bool isReverseU = false)
        {
            if (UpperZ - V > 0.1)
                return;

            if (point.Z > UpperZ)
                point = point.WithZ(UpperZ);

            if (UpperZ - point.Z > 0.1)
            {
                Move(point.WithZ(UpperZ), direction, isReverseAngle, isReverseU);
                isReverseAngle = false;
                isReverseU = false;
            }

            GCommands(0, point.ToPoint2d(), (point + direction).ToPoint2d(), point.Z, isReverseAngle, isReverseU);
        }

        public void Move(Point2d point1, Point2d point2, bool isReverseAngle = false, bool isReverseU = false)
        {
            GCommands(0, point1, point2, null, isReverseAngle, isReverseU);
        }

        public void Cutting(Point3d point, Vector3d direction) => Cutting(point.ToPoint2d(), (point + direction).ToPoint2d(), point.Z);
        
        public void Cutting(Line3d line) => Cutting(line.StartPoint, line.EndPoint);

        public void Cutting(Point3d point1, Point3d point2) => Cutting(point1.To2d(), point2.To2d(), (point1.Z + point2.Z) / 2);

        public void Cutting(Point2d point1, Point2d point2, double z)
        {
            if (!IsEngineStarted)
            {
                AddCommands(_postProcessor.StartEngine());
                IsEngineStarted = true;
            }
            GCommands(1, point1, point2, z, false, false);
        }

        public void GCommands(int gCode, Point2d point1, Point2d point2, double? z, bool isReverseAngle, bool isReverseU)
        {
            var line = new Line2d(point1, point2);
            var point = line.GetClosestPointTo(Center).Point;
            var centerVector = point - Center;
            var centerDist = centerVector.Length.Round(3);
            if (centerDist == 0)
                centerVector = line.Direction.GetPerpendicularVector();
            var uSign = centerVector.DotProduct(_uAxis).GetSign();
            uSign *= isReverseU.GetSign(-1);

            GCommandA(centerVector * uSign, isReverseAngle, line.Direction.Angle);

            GCommandUV(gCode, centerDist * uSign, z ?? V, point);
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
            var toolVector = Vector3d.XAxis.RotateBy(_toolAngle, Vector3d.ZAxis) * ToolLength;
            _toolAngle = newToolAngle;

            AddCommand($"G05 A{da} S{_processing.S}", "Rotate", CreateToolpath(toolVector), CreateToolpath(-toolVector));

            _uAxis = _uAxis.RotateBy(daRad);
            _operationDuration += Math.Abs(da) / _processing.S * 60;

            return;

            ObjectId CreateToolpath(Vector3d vector1) => _toolpathBuilder.AddToolpath(NoDraw.ArcSCA(_toolPoint + vector1, Center.WithZ(_toolPoint.Z), daRad));
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

            var toolVector = Vector3d.XAxis.RotateBy(_toolAngle, Vector3d.ZAxis) * ToolLength;
            var newToolPoint = point.WithZ(v);
            var toolpath1 = CreateToolpath(toolVector);
            var toolpath2 = CreateToolpath(-toolVector);
            _toolPoint = newToolPoint;

            AddCommand(commandText, "", toolpath1, toolpath2);

            _operationDuration += Math.Sqrt(du * du + dv * dv) / (gCode == 0 ? 500 : Feed) * 60;
            
            return;

            ObjectId CreateToolpath(Vector3d vector) => _toolpathBuilder.AddToolpath(NoDraw.Line(_toolPoint + vector, newToolPoint + vector));
        }
    }
}