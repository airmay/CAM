using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;

namespace CAM
{
    /// <summary>
    /// Генератор команд для станка типа Krea
    /// </summary>
    public class DonatoniCommandGenerator
    {
        private int _startRangeIndex;

        public List<ProcessCommand> Commands { get; } = new List<ProcessCommand>();

        /// <summary>
        /// Запуск станка
        /// </summary>
        /// <param name="toolNumber"></param>
        public void StartMachine(string caption, int toolNumber)
        {
            CreateCommand($"; Donatoni \"{caption}\"");
            CreateCommand($"; DATE {DateTime.Now}");

            //if (Settings.Machine == MachineKind.Krea)
            //CreateCommand("(UAO,E30)");
            //CreateCommand("(UIO,Z(E31))");

            CreateCommand("%300");
            CreateCommand("RTCP=1");
            CreateCommand("G600 X0 Y-2500 Z-370 U3800 V0 W0 N0");
            CreateCommand("G601");
            CreateCommand("G0 G53 Z0.0");
            CreateCommand("G0 G53 A0");
            CreateCommand("G64");
            CreateCommand("G154O10");
        }

        /// <summary>
        /// Остановка станка
        /// </summary>
        public void StopMachine()
        {
            CreateCommand("M5");              // выключение шпинделя                       
            //if (Settings.Machine == MachineKind.Krea)
            //{
            //    AddLine("M9 M10");          // выключение воды
            //    AddLine("G0 G79 Z(@ZUP)");  // подъем в верхнюю точку
            //}
            CreateCommand("M9");
            CreateCommand("G61");
            CreateCommand("G153");
            CreateCommand(";G0 G53 Z0");
            CreateCommand("SETMSP=1");
            CreateCommand("G0 G53 Z0 ");
            CreateCommand("G0 G53 A0 C0");
            CreateCommand("G0 G53 X0 Y0");
            CreateCommand("M30");

            int number = 0;
            Commands.ForEach(p => p.Number = ++number);
        }

        /// <summary>
        /// Первый подвод
        /// </summary>
        /// <param name="point"></param>
        /// <param name="angle"></param>
        /// <param name="frequency"></param>
        public void InitialMove(Point3d point, double angle, int frequency) //, int feed)
        {
            CreateCommand("T1");
            CreateCommand("M6");
            CreateCommand("G172 T1 H1 D1");
            CreateCommand("M300");
            //G17 плоскость

            CreateCommand($"G0 X{Round(point.X)} Y{Round(point.Y)} C{angle}", endPoint: point, toolAngle: angle);
            CreateCommand($"G0 Z{Round(point.Z)}", endPoint: point, toolAngle: angle);

            //CreateCommand($"G1 A0 F{feed}");
            //CreateCommand($"G1 Z{Round(point.Z)} F{feed}");

            CreateCommand("M8"); // M7 ?
            CreateCommand($"M3 S{frequency}");
        }

        public void StartRange() => _startRangeIndex = Commands.Count;

        public List<ProcessCommand> GetRange() => Commands.GetRange(_startRangeIndex, Commands.Count - _startRangeIndex);

        /// <summary>
        /// Быстрая подача
        /// </summary>
        /// <param name="line"></param>
        /// <param name="angle"></param>
        public void Fast(Line line, double angleC, double angleA = 0)
        {
            CreateCommand(CommandNames.Fast, 0, toolpathCurve: line, endPoint: line.EndPoint, toolAngle: angleC);
        }

        /// <summary>
        /// Поднятие
        /// </summary>
        /// <param name="line"></param>
        public void Uplifting(Line line, double angle) => CreateCommand(CommandNames.Uplifting, 0, toolpathCurve: line, endPoint: line.EndPoint, toolAngle: angle);

        /// <summary>
        /// Рез
        /// </summary>
        public void Cutting(string name, Curve curve, int feed, Point3d point, double angle)
        {
            switch (curve)
            {
                case Line _:
                    CreateCommand(name, 1, feed: feed, toolpathCurve: curve, endPoint: point, toolAngle: angle);
                    break;
                case Arc arc:
                    var code = point == curve.StartPoint ? 2 : 3;
                    CreateCommand(name, code, feed: feed, toolpathCurve: curve, endPoint: point, toolAngle: angle);
                    break;
                default:
                    throw new Exception($"Неподдерживаемый тип кривой {curve.GetType()}");
            }
        }

        //private void CreateCommand(string name, int gCode, int? mCode = null, string axis = null, int? feed = null, double? x = null, double? y = null,
        //    double? param1 = null, double? param2 = null, Curve toolpathCurve = null, Point3d? endPoint = null, double? toolAngle = null)
        //    => Commands.Add(new ProcessCommand
        //    {
        //        Name = name,
        //        GCode = gCode.ToString(),
        //        MCode = mCode?.ToString(),
        //        Axis = axis,
        //        Feed = feed.ToString(),
        //        X = Round(x),
        //        Y = Round(y),
        //        Param1 = Round(param1),
        //        Param2 = Round(param2),
        //        ToolpathCurve = toolpathCurve,
        //        EndPoint = endPoint,
        //        ToolAngle = toolAngle
        //    });

        private void CreateCommand(string text, Curve toolpathCurve = null, Point3d? endPoint = null, double? toolAngle = null) => Commands.Add(new ProcessCommand
        {
            Text = text,
            ToolpathCurve = toolpathCurve,
            EndPoint = endPoint,
            ToolAngle = toolAngle
        });

        private void CreateCommand(string name, int gCode, int? feed = null, Curve toolpathCurve = null, Point3d? endPoint = null, double? toolAngle = null) => Commands.Add(new ProcessCommand
        {
            Name = name,
            Text = $"G{gCode}{Format('X', endPoint?.X)}{Format('Y', endPoint?.Y)}{Format('Z', endPoint?.Z)}{Format('F', feed)}",
            ToolpathCurve = toolpathCurve,
            EndPoint = endPoint,
            ToolAngle = toolAngle
        });

        private static string Format(char label, double? value) => value.HasValue ? $" {label}{Math.Round(value.Value, 4)}" : String.Empty;

        private static double Round(double value) => Math.Round(value, 4);

    }
}
