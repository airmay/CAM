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
        public Point3d? CenterToolPosition { get; set; }
        public Point3d? PointToolPosition { get; set; }
        public double? AngleToolPosition { get; set; }

        public CableToolPosition GetToolPosition()
        {
            return CenterToolPosition.HasValue && PointToolPosition.HasValue && AngleToolPosition.HasValue 
                ? new CableToolPosition(PointToolPosition.Value, CenterToolPosition.Value, AngleToolPosition.Value)
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

        protected string GCommandText(int gCode, string paramsString, Point3d point, Curve curve, double? angleC, double? angleA, int? feed, Point2d? center)
        {
            return $"G{gCode} U{point.X.Round(4)} V{point.Y.Round(4)}";
        }
    }
}
