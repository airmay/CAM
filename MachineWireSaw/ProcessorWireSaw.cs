using System;
using System.Diagnostics;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.CncWorkCenter;
using CAM.Core;
using Dreambuild.AutoCAD;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CAM.MachineWireSaw
{
    public class ProcessorWireSaw : IProcessor
    {
        private readonly PostProcessorWireSaw _postProcessor;
        private readonly ProcessingWireSaw _processing;
        public bool IsEngineStarted;
        private IOperation _operation;
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

        public void Dispose()
        {
        }

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

        public void AddCommand(string text, string name = null)
        {
            if (text == null)
                return;

            var x = Center.X - U * Math.Cos(Angle.ToRad());
            var y = Center.Y - U * Math.Sin(Angle.ToRad());
            var toolLocationParams = new ToolLocationParams(x, y, V, Angle, 0);

            ProcessingBase.Program.AddCommand(new Command
            {
                Name = name,
                Text = text,
                ToolLocationParams = toolLocationParams,
                Operation = _operation,
            });
        }

        private void AddCommands(string[] commands)
        {
            Array.ForEach(commands, p => AddCommand(p));
        }

        public double Angle;
        public double U { get; set; }
        public double DU { get; set; }
        public double V { get; set; }
        public int Feed => _processing.CuttingFeed;

        public void StartOperation(double angle, double u, double v)
        {
            if (IsEngineStarted)
                return;

            IsEngineStarted = true;
            Angle = angle;
            U = u;
            V = v;

            AddCommands(_postProcessor.StartMachine());
        }

        public Point2d Center => _processing.Origin.Point;
        public bool IsExtraMove { get; set; }
        public bool IsExtraRotate { get; set; }
        private Vector2d _normal = Vector2d.XAxis;
        private double _signU = -1;
        private Point2d _point;
        private double _angle;

        public void Pause(double duration)
        {
            AddCommand("(DLY,{duration})", "Пауза");
        }

        public void GCommand(int gCode, Line3d line3d, bool isReverseAngle = false)
        {
            GCommand(gCode, new Line2d(line3d.PointOnLine.To2d(), line3d.Direction.ToVector2d()), line3d.PointOnLine.Z,
                isReverseAngle);
        }

        public void GCommand(int gCode, Point3d point1, Point3d point2, bool isRevereseAngle = false)
        {
            GCommand(gCode, new Line2d(point1.To2d(), point2.To2d()), (point1.Z + point2.Z) / 2, isRevereseAngle);
        }

        private int _uSign = -1;   // положение троса: -1 - до центра вращения, 1 - после
        private int _startNormalAngle;
        private Point2d _toolPoint;

        private int _startAngle;

        public void Move(Line2d line, bool isReverseAngle, bool isReverseU)
        {
            var da = 0D;
            if (!line.IsOn(Center))
            {
                _toolPoint = line.GetClosestPointTo(Center).Point;
                var normal = _toolPoint - Center;
                if (isReverseU)
                {
                    _normal = _normal.Negate();
                    _signU *= -1;
                }
                da = _normal.MinusPiToPiAngleTo(normal).ToRoundDeg();
            }
            else
            {
                _toolPoint = Center;
                var angle = line.Direction.Angle.ToRoundDeg() % 180;
                da = angle - 90;
            }

            if (isReverseAngle)
                da -= 360 * Math.Sign(da);

            GcommandA(angle);

            var u = 0D;
            if (!line.IsOn(Center))
            {
                _toolPoint = line.GetClosestPointTo(Center).Point;
                var normal = _toolPoint - Center;
                if (normal.DotProduct(_normal) < 0)
                    _signU *= -1;
                u = normal.Length.Round(3) * _signU;
                _normal = normal;
            }
            else
            {
                _toolPoint = Center;
            }

            GCommandUV(1, u, z.Round(3));
        }

        public void GcommandA(double da)
        {
            AddCommand($"G05 A{da} S{_processing.S}", "Rotate");
            var duration = Math.Abs(da) / _processing.S * 60;
        }

        public void Cutting(Line2d line, double z)
        {
            var angle = line.Direction.Angle.ToRoundDeg() % 180;
            GCommandA(angle);

            var u = 0D;
            if (!line.IsOn(Center))
            {
                _toolPoint = line.GetClosestPointTo(Center).Point;
                var normal = _toolPoint - Center;
                if (normal.DotProduct(_normal) < 0)
                    _signU *= -1;
                u = normal.Length.Round(3) * _signU;
                _normal = normal;
            }
            else
            {
                _toolPoint = Center;
            }

            GCommandUV(1, u, z.Round(3));
        }

        public void GCommandA(double angle)
        {
            var da = angle - Angle;
            if (da == 0)
                return;

            var abs = Math.Abs(da);
            if (abs > 90)
                da = (180 - abs) * Math.Sign(-da);

            AddCommand($"G05 A{da} S{_processing.S}", "Rotate");
            var duration = abs / _processing.S * 60;
        }

        public void GCommandUV(int gCode, double u, double v)
        {
            var du = u - U;
            var dv = v - V;
            if (du == 0 && dv == 0)
                return;

            var text = $"G0{gCode} U{du} V{dv}";
            if (gCode == 1)
                text += $" F{Feed}";

            var duration = Math.Sqrt(du * du + dv * dv) / (gCode == 0 ? 500 : Feed) * 60;

            AddCommand(text);
            U = u;
            V = v;
        }

        public void GCommand(int gCode, Line2d line2d, double z, bool isReverseAngle)
        {
            var angle = line2d.Direction.Angle.ToRoundDeg() % 180;
            var da = angle - _angle;
            if (Math.Abs(da) > 90) // переход через ось Х
                da = (180 - Math.Abs(da)) * Math.Sign(-da);
            _angle = angle;

            var normal = !line2d.IsOn(Center) ? line2d.GetClosestPointTo(Center).Point - Center : new Vector2d();
            //var normalAngle = normal.
            //var u = normal.Length.Round(3);
            var dn = Math.Abs(normal.Angle - _normal.Angle).ToDeg();
            if (dn > 90 && dn < 270) // переход через центр
            {

            }


            if (!_normal.IsZeroLength() && !normal.IsZeroLength())
            {
                if (Math.Abs(angle - Angle) == 90)
                    angle = Angle + normal.MinusPiToPiAngleTo(_normal) > 0 ? 90 : -90;
                else if (normal.GetAngleTo(_normal) > Math.PI / 2)
                    _signU *= -1;
            }

            if (_normal.IsZeroLength())
                _signU = -Math.Sign(angle) * Math.Sign(normal.Y);

            if (_signU == 0)
                _signU = -1;

            if (gCode == 0 && Math.Abs(angle - Angle) > 0.01 && IsExtraRotate)
                GCommandA(angle + Math.Sign(angle - Angle));

            GCommandA(angle);

            var u = (_signU * normal.Length).Round(4);
            var v = z.Round(4);

            if (gCode == 0)
            {
                if (IsExtraMove)
                    GCommandUV(0, u + Math.Sign(u - U) * 20, V);
                GCommandUV(0, u, V);
                GCommandUV(0, u, v);
            }
            else
            {
                GCommandUV(1, u, v);
            }

            _normal = normal;

            if (!IsEngineStarted)
            {
                AddCommands(_postProcessor.StartEngine());
                IsEngineStarted = true;
            }
        }

        public void GCommand1(int gCode, Line2d line2d, double z, bool isReverseAngle)
        {
            var angle = Vector2d.YAxis.MinusPiToPiAngleTo(line2d.Direction).ToDeg(4);
            var da = Math.Sign(Angle - angle) * 180;
            while (Math.Abs(angle - Angle) > 90)
                angle += da;
            angle = angle.Round(4);

            if (isReverseAngle)
                angle += -Math.Sign(angle) * 180;

            var normal = !line2d.IsOn(Center) ? line2d.GetClosestPointTo(Center).Point - Center : new Vector2d();
            if (!_normal.IsZeroLength() && !normal.IsZeroLength())
            {
                if (Math.Abs(angle - Angle) == 90)
                    angle = Angle + normal.MinusPiToPiAngleTo(_normal) > 0 ? 90 : -90;
                else if (normal.GetAngleTo(_normal) > Math.PI / 2)
                    _signU *= -1;
            }

            if (_normal.IsZeroLength())
                _signU = -Math.Sign(angle) * Math.Sign(normal.Y);

            if (_signU == 0)
                _signU = -1;

            if (gCode == 0 && Math.Abs(angle - Angle) > 0.01 && IsExtraRotate)
                GCommandA(angle + Math.Sign(angle - Angle));

            GCommandA(angle);

            var u = (_signU * normal.Length).Round(4);
            var v = z.Round(4);

            if (gCode == 0)
            {
                if (IsExtraMove)
                    GCommandUV(0, u + Math.Sign(u - U) * 20, V);
                GCommandUV(0, u, V);
                GCommandUV(0, u, v);
            }
            else
            {
                GCommandUV(1, u, v);
            }

            _normal = normal;

            if (!IsEngineStarted)
            {
                AddCommands(_postProcessor.StartEngine());
                IsEngineStarted = true;
            }
        }

        public void GCommandA1(double angle)
        {
            if (Math.Abs(angle - Angle) < 0.001)
                return;

            var da = (angle - Angle).Round(4);
            Angle += da;

            var text = $"G05 A{da} S{_processing.S}";
            var duration = Math.Abs(da) / _processing.S * 60;

            AddCommand(text, "Rotate");
        }

        public void GCommand(int gCode, Line2d line, double z)
        {
            var angle = line.Direction.Angle.ToRoundDeg() % 180;
            var da = angle - Angle;
            if (da > 90)
                da -= 180;
            if (da < -90)
                da += 180;

            var normal = line.GetClosestPointTo(Center).Point - Center;
            if (normal.GetAngleTo(_normal) > Math.PI / 2)
                _signU *= -1;
        }
    }
}