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

        public void SetToolPosition(Point3d centerPoint, double angle, double u, double v)
        {
            CenterPoint = centerPoint;
            //Angle = angle;
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
        }

        protected override void StopEngineCommands()
        {
        }

        public void Pause(double duration) => Command(string.Format(CultureInfo.InvariantCulture, "(DLY,{0})", duration), "Пауза", duration);

        public void GCommand(int gCode, double u, double? v = null, int? feed = null, string name = "")
        {
            var ur = u.Round(6);
            var vr = v?.Round(6) ?? V;
            if (ur == U && vr == V)
                return;
            var text = $"G0{gCode} U{(ur - U).Round(4)} V{(vr - V).Round(4)} {(feed.HasValue ? "F" + feed.ToString() : null)}";
            U = ur;
            V = vr;
            Command(text, name);
        }

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
        private Vector2d _normal;
        private double _angle = 0;
        private double _signU;

        public void GCommand(Point3d point1, Point3d point2, int gCode)
        {
            var line2d = new Line2d(point1.To2d(), point2.To2d());
            var angle = Vector2d.YAxis.MinusPiToPiAngleTo(line2d.Direction).ToDeg(4);
            var da = Math.Sign(_angle - angle) * 180;
            while (Math.Abs(angle - _angle) > 90)
                angle += da;
            angle = angle.Round(4);

            var normal = !line2d.IsOn(Center) ? line2d.GetClosestPointTo(Center).Point - Center : new Vector2d();
            if (!_normal.IsZeroLength() && !normal.IsZeroLength())
            {
                if (Math.Abs(angle) == 90)
                    angle = normal.MinusPiToPiAngleTo(_normal) > 0 ? 90 : -90;
                else
                    if (normal.GetAngleTo(_normal) > Math.PI / 2)
                        _signU *= -1;
            }
            if (_normal.IsZeroLength())
                _signU = Math.Sign(angle);

            if (angle != _angle)
            {
                var text = $"G05 A{(angle - _angle).Round(4)} S{S}";
                _angle = angle;
                Command(text, "Rotate");
            }

            var u = (_signU * normal.Length).Round(4);
            var v = ((point1.Z + point2.Z) / 2).Round(4);
            if (u != U || v != V)
            {
                var text = $"G0{gCode} U{(u - U).Round(4)} V{(v - V).Round(4)} {(gCode == 1 ? "F" + Feed : null)}";
                U = u;
                V = v;
                Command(text);
            }
            _normal = normal;
        }
    }
}