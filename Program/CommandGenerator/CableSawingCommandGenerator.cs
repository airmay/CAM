using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Globalization;

namespace CAM
{
    /// <summary>
    /// Генератор команд для тросикового станка
    /// </summary>
    [MachineType(MachineType.CableSawing)]
    public class CableCommandGenerator : CommandGeneratorBase
    {
        const double Epsilon = 0.000001;

        public Point3d CenterPoint { get; set; }
        public Vector2d Vector { get; set; } = Vector2d.YAxis;

        public double U { get; set; }
        public double V { get; set; }
        public int Feed { get; set; }
        public int S { get; set; }

        private bool _isSetPosition;
        public bool _isEngineStarted = false;

        public void SetToolPosition(Point3d centerPoint, double angle, double u, double v)
        {
            Center = centerPoint.To2d();
            _angle = angle;
            U = u.Round(6);
            V = v.Round(6);
            _isSetPosition = true;
        }

        private CableToolPosition GetToolPosition()
        {
            var point = new Point3d(Center.X - U, Center.Y, V);
            var angle = _angle.ToRad(); // Vector2d.YAxis.MinusPiToPiAngleTo(Vector);
            return new CableToolPosition(point, Center.ToPoint3d(), angle);
        }

        public void Command(string text, string name = null, double duration = 0)
        {
            AddCommand(new ProcessCommand
            {
                Name = name,
                Text = text,
                //HasTool = _hasTool,
                ToolLocation = GetToolPosition(),
                Duration = duration,
                U = U,
                V = V,
                //A =  Vector2d.YAxis.MinusPiToPiAngleTo(Vector).ToDeg(6)
                A = _angle
            });
        }


        public CableCommandGenerator()
        {
        }

        protected override void StartMachineCommands(string caption)
        {
        }

        protected override void StopMachineCommands()
        {
        }

        protected override void SetToolCommands(int toolNo, double angleA)
        {
        }

        protected override void StartEngineCommands()
        {
            Command($"M03", "Включение");
            _isEngineStarted = true;
        }

        protected override void StopEngineCommands()
        {
        }

        public void Pause(double duration) => Command(string.Format(CultureInfo.InvariantCulture, "(DLY,{0})", duration), "Пауза", duration);

        //public void GCommand(int gCode, double u, double? v = null, int? feed = null, string name = "")
        //{
        //    var ur = u.Round(6);
        //    var vr = v?.Round(6) ?? V;
        //    if (ur == U && vr == V)
        //        return;
        //    var text = $"G0{gCode} U{(ur - U).Round(4)} V{(vr - V).Round(4)} {(feed.HasValue ? "F" + feed.ToString() : null)}";
        //    U = ur;
        //    V = vr;
        //    Command(text, name);
        //}

        public void GCommandAngle(Vector2d vector, int s)
        {
            var angle = Vector.MinusPiToPiAngleTo(vector);
            if (Math.Abs(angle.ToDeg(6)) >= 90)
            {
                vector = vector.Negate();
                angle = Vector.MinusPiToPiAngleTo(vector);
            }
            if (Math.Abs(angle.ToDeg()) < 0.000001)
                return;

            //var da = angle - Angle;
            //if (da > 180)
            //    da -= 360;
            //if (da < -180)
            //    da += 360;

            Vector = vector;
            Command($"G05 A{angle.ToDeg(6)} S{s}", "Rotate");
            //Command($"G05 A{angle.ToDeg(6)} S{s} A={Vector.Angle.ToDeg(6)} U={U.Round(6)}", "Rotate");
        }

        public Point2d Center { get; set; }
        public bool IsExtraMove { get; set; }
        public bool IsExtraRotate { get; set; }
        private Vector2d _normal;
        private double _angle = 0;
        private double _signU;

        public void GCommand(int gCode, Line2d line2d, double z, bool isRevereseAngle)
        {
            var angle = Vector2d.YAxis.MinusPiToPiAngleTo(line2d.Direction).ToDeg(4);
            var da = Math.Sign(_angle - angle) * 180;
            while (Math.Abs(angle - _angle) > 90)
                angle += da;
            angle = angle.Round(4);

            if (isRevereseAngle)
                angle += -Math.Sign(angle) * 180;

            var normal = !line2d.IsOn(Center) ? line2d.GetClosestPointTo(Center).Point - Center : new Vector2d();
            if (!_normal.IsZeroLength() && !normal.IsZeroLength())
            {
                if (Math.Abs(angle - _angle) == 90)
                    angle = _angle + normal.MinusPiToPiAngleTo(_normal) > 0 ? 90 : -90;
                else
                    if (normal.GetAngleTo(_normal) > Math.PI / 2)
                        _signU *= -1;
            }
            if (_normal.IsZeroLength())
                _signU = -Math.Sign(angle) * Math.Sign(normal.Y);

            if (_signU == 0)
                _signU = -1;

            if (gCode == 0 && Math.Abs(angle - _angle) > 0.01 && IsExtraRotate)
                GCommandA(angle + Math.Sign(angle - _angle));
            
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

            if (!_isEngineStarted)
            {
                StartEngineCommands();
            }
        }

        public void GCommandUV(int gCode, double u, double v)
        {
            if (u == U && v == V)
                return;

            var du = -(u - U).Round(4);
            var dv = (v - V).Round(4);
            U = u;
            V = v;

            var feed = gCode == 1 ? $"F{Feed}" : "";
            var text = $"G0{gCode} U{du} V{dv} {feed}";
            var duration = Math.Sqrt(du * du + dv * dv) / (gCode == 0 ? 500 : Feed) * 60;

            Command(text, duration: duration);
        }

        public void GCommandA(double angle)
        {
            if (Math.Abs(angle - _angle) < 0.01)
                return;
            
            var da = (angle - _angle).Round(4);
            _angle = angle;

            var text = $"G05 A{da} S{S}";
            var duration = Math.Abs(da) / S * 60;

            Command(text, "Rotate", duration);
        }


        public void GCommand(int gCode, Line3d line3d, bool isRevereseAngle = false) => GCommand(gCode, new Line2d(line3d.PointOnLine.To2d(), line3d.Direction.ToVector2d()), line3d.PointOnLine.Z, isRevereseAngle);

        public void GCommand(int gCode, Point3d point1, Point3d point2, bool isRevereseAngle = false) => GCommand(gCode, new Line2d(point1.To2d(), point2.To2d()), (point1.Z + point2.Z) / 2, isRevereseAngle);
    }
}