using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Globalization;

namespace CAM
{
    /// <summary>
    /// Генератор команд для станка типа Krea
    /// </summary>
    [MachineType(MachineType.Krea)]
    public class KreaCommandGenerator : CommandGeneratorBase
    {
        protected override void StartMachineCommands(string caption)
        {
            Command($"; Krea \"{caption}\"");
            Command($"; DATE {DateTime.Now}");
            Command("(UAO,E30)");
            Command("(UIO,Z(E31))");
        }

        protected override void StopMachineCommands()
        {
            Command("G0 G79 Z(@ZUP)", "Подъем");
            Command("M30", "Конец");
        }

        protected override void SetToolCommands(int toolNo, double angleA, double angleC, int originCellNumber)
        {
            Command($"T1", "Инструмент№");
            Command("G17", "Плоскость");
            Command("G79Z(@ZUP)");
        }

        protected override void StartEngineCommands()
        {
            Command("M7", "Охлаждение");
            Command("M8", "Охлаждение");
            Command($"S{_frequency}", "Шпиндель");
            Command($"M3", "Шпиндель");
        }

        protected override void StopEngineCommands()
        {
            Command("M5", "Шпиндель откл.");
            Command("M9 M10", "Охлаждение откл.");
        }

        public override void Pause(double duration) => Command(string.Format(CultureInfo.InvariantCulture, "(DLY,{0})", duration), "Пауза", duration);

        protected override string GCommandText(int gCode, string paramsString, Point3d point, Curve curve, double? angleC, double? angleA, int? feed, Point2d? center)
        {
            return $"G{gCode}{Format("X", point.X, ToolLocation.Point.X, _originX)}{Format("Y", point.Y, ToolLocation.Point.Y, _originY)}" +
                $"{FormatIJ("I", center?.X, _originX)}{FormatIJ("J", center?.Y, _originY)}" +
                $"{Format("Z", point.Z, ToolLocation.Point.Z, withThick: WithThick)}{Format("C", angleC, ToolLocation.AngleC)}{Format("A", angleA, ToolLocation.AngleA)}" +
                $"{Format("F", feed, _feed)}";

            string Format(string label, double? value, double oldValue, double origin = 0, bool withThick = false) =>
                 (paramsString == null || paramsString.Contains(label)) && (value.GetValueOrDefault(oldValue) != oldValue) // || (_GCode == 0 ^ gCode == 0))
                        ? $" {label}{FormatValue((value.GetValueOrDefault(oldValue) - origin).Round(4), withThick)}"
                        : null;

            string FormatIJ(string label, double? value, double origin) => value.HasValue ? $" {label}{(value.Value - origin).Round(4)}" : null;

            string FormatValue(double value, bool withThick) => withThick ? $"({value} + THICK)" : value.ToString();
        }
    }
}
