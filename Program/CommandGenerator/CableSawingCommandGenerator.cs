using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Globalization;

namespace CAM.Program.CommandGenerator
{
    /// <summary>
    /// Генератор команд для тросикового станка
    /// </summary>
    [MachineType(MachineType.CableSawing)]
    public class CableSawingCommandGenerator : CommandGeneratorBase
    {
        public CableSawingCommandGenerator()
        {
            _hasTool = true;
        }

        protected override void StartMachineCommands(string caption)
        {
        }

        protected override void StopMachineCommands()
        {
        }

        protected override void SetToolCommands(int toolNo, double angleA, double angleC, int originCellNumber)
        {
        }

        protected override void StartEngineCommands()
        {
        }

        protected override void StopEngineCommands()
        {
        }

        public override void Pause(double duration) => Command(string.Format(CultureInfo.InvariantCulture, "(DLY,{0})", duration), "Пауза", duration);

        protected override string GCommandText(int gCode, string paramsString, Point3d point, Curve curve, double? angleC, double? angleA, int? feed, Point2d? center)
        {
            return $"G{gCode} U{point.X.Round(4)} V{point.Y.Round(4)}";
        }
    }
}
