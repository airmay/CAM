using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Globalization;

namespace CAM
{
    /// <summary>
    /// Генератор команд для тросикового станка
    /// </summary>
    [MachineType(MachineType.CableSawing)]
    public class CableCommandGenerator : CommandGeneratorBase
    {
        public Point3d CenterPoint { get; set; }
        public double Angle { get; set; }

        public double U { get; set; }
        public double V { get; set; }

        private bool _isSetPosition;

        public void SetToolPosition(Point3d centerPoint, double angle, double u, double v)
        {
            CenterPoint = centerPoint;
            Angle = angle;
            U = u;
            V = v;
            _isSetPosition = true;
        }

        private CableToolPosition GetToolPosition()
        {
            return _isSetPosition
                ? new CableToolPosition(new Point3d(CenterPoint.X - U, CenterPoint.Y, V), CenterPoint, Angle)
                : null;
        }

        public void Command(string text, string name = null, double duration = 0)
        {
            AddCommand(new ProcessCommand
            {
                Name = name,
                Text = text,
                //HasTool = _hasTool,
                ToolLocation = GetToolPosition(),
                Duration = duration
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

        public void GCommand(int gCode, double u, double v, int? feed = null, string name = "")
        {
            var text = $"G0{gCode} U{(u - U).Round(4)} V{(v - V).Round(4)} {(feed.HasValue ? "F" + feed.ToString() : null)}";
            U = u;
            V = v;
            Command(text, name);
        }

        public void GCommandAngle(double angle, int s)
        {
            var da = angle - Angle;
            if (da > 180)
                da -= 360;
            if (da < -180)
                da += 360;

            Angle = angle;
            Command($"G05 A{da} S{s}", "Rotate");
        }
    }
}
