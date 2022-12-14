using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CAM.Commands
{
    /// <summary>
    /// Генератор команд для станка типа ScemaLogic
    /// </summary>
    [MachineType(MachineType.ScemaLogic)]
    public class ScemaLogicCommandGenerator : MillingCommandGenerator
    {
        protected override void StartMachineCommands(string caption)
        {
            //Command($"; ScemaLogic \"{caption}\"");
            //Command($"; DATE {DateTime.Now}");

            Command("98;;;;;;;;");
            Command("97;2;;1;;;;;");
            Command("17;;XYCZ;;;;;;");
            Command("28;;XYCZ;;;;;;");
        }

        protected override void StopMachineCommands()
        {
            Command("97;30;;;;;;;", "Конец");
        }

        protected override void SetToolCommands(int toolNo, double angleA, double angleC, int originCellNumber)
        {
            Command($"97;6;;{toolNo};;;;;", "Инструмент№");
        }

        public override void StartEngineCommands()
        {
            Command("97;7;;;;;;;", "Охлаждение");
            Command("97;8;;;;;;;", "Охлаждение");
            Command($"97;3;;{_frequency};;;;;", "Шпиндель");
            //CreateCommand(CommandNames.Cycle, 28, axis: "XYCZ");
        }

        protected override void StopEngineCommands()
        {
            Command("97;9;;;;;;;", "Охлаждение откл.");
            Command("97;10;;;;;;;");
            Command("97;5;;;;;;;", "Шпиндель откл.");
        }

        protected override string GCommandText(int gCode, string paramsString, MillToolPosition position, Point3d point, Curve curve, double? angleC, double? angleA, int? feed, Point2d? center)
        {
            var text = $"{(point.X - _originX).Round(4)};{(point.Y - _originY).Round(4)};";
            if (gCode == 0)
                return angleC.HasValue ? $"0;;XYC;0;{text}{angleC.Value.Round(4)};;" : $"0;;XYZ;0;{text}{point.Z.Round(4)};;";

            if (center!= null)
                text += $"{(center.Value.X - _originX).Round(4)};{(center.Value.Y - _originY).Round(4)};";
            else
                text += $"{(angleC ?? ToolPosition.AngleC).Round(4)};{point.Z.Round(4)};";

            return $"{gCode};;XYCZ;{feed ?? _feed};{text}";
        }

        public override void Cycle() => Command("28;;XYCZ;;;;;;", CommandNames.Cycle);
    }
}
