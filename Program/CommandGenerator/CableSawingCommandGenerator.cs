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
            var point = new Point3d(CenterPoint.X - U, CenterPoint.Y, V);
            var angle = Vector2d.YAxis.MinusPiToPiAngleTo(Vector);
            return new CableToolPosition(point, CenterPoint, angle);
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
                U = U.Round(6),
                V = V.Round(6),
                A = Vector2d.YAxis.MinusPiToPiAngleTo(Vector).ToDeg(6)
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
    }
}
