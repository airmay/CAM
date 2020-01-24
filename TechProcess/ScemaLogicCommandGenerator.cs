using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;

namespace CAM
{
    /// <summary>
    /// Генератор команд для станка типа ScemaLogic
    /// </summary>
    public class ScemaLogicCommandGenerator
    {
        private int _startRangeIndex;

        public List<ProcessCommand> Commands { get; } = new List<ProcessCommand>();

        /// <summary>
        /// Запуск станка
        /// </summary>
        /// <param name="toolNumber"></param>
        public void StartMachine(int toolNumber)
        {
            var name = "";
            CreateCommand(name, 98);
            CreateCommand(name, 97, 2, feed: 1);
            CreateCommand(name, 17, axis: "XYCZ");
            CreateCommand(name, 28, axis: "XYCZ");
            CreateCommand(name, 97, 6, feed: toolNumber);
        }

        /// <summary>
        /// Остановка станка
        /// </summary>
        public void StopMachine()
        {
            var name = "";
            CreateCommand(name, 97, 9);
            CreateCommand(name, 97, 10);
            CreateCommand(name, 97, 5);
            CreateCommand(name, 97, 30);

            int number = 0;
            Commands.ForEach(p => p.Number = ++number);
        }

        /// <summary>
        /// Первый подвод
        /// </summary>
        /// <param name="point"></param>
        /// <param name="angle"></param>
        /// <param name="frequency"></param>
        public void InitialMove(Point3d point, double angle, int frequency)
        {
            CreateCommand(CommandNames.InitialMove, 0, axis: "XYC", feed: 0, x: point.X, y: point.Y, param1: angle);
            CreateCommand(CommandNames.InitialMove, 0, axis: "XYZ", feed: 0, x: point.X, y: point.Y, param1: point.Z, endPoint: point, toolAngle: angle);
            CreateCommand("", 97, 7);
            CreateCommand("", 97, 8);
            CreateCommand("", 97, 3, feed: frequency);
            CreateCommand(CommandNames.Cycle, 28, axis: "XYCZ");
        }

        public void StartRange() => _startRangeIndex = Commands.Count;

        public List<ProcessCommand> GetRange() => Commands.GetRange(_startRangeIndex, Commands.Count - _startRangeIndex);

        /// <summary>
        /// Быстрая подача
        /// </summary>
        /// <param name="line"></param>
        /// <param name="angle"></param>
        public void Fast(Line line, double angle)
        {
            CreateCommand(CommandNames.Fast, 0, axis: "XYC", feed: 0, x: line.EndPoint.X, y: line.EndPoint.Y, param1: angle);
            CreateCommand(CommandNames.Fast, 0, axis: "XYZ", feed: 0, x: line.EndPoint.X, y: line.EndPoint.Y, param1: line.EndPoint.Z, toolpathCurve: line, endPoint: line.EndPoint, toolAngle: angle);
            CreateCommand(CommandNames.Cycle, 28, axis: "XYCZ");
        }

        /// <summary>
        /// Поднятие
        /// </summary>
        /// <param name="line"></param>
        public void Uplifting(Line line, double angle) => CreateCommand(CommandNames.Uplifting, 0, axis: "XYZ", feed: 0, x: line.EndPoint.X, y: line.EndPoint.Y, param1: line.EndPoint.Z, toolpathCurve: line, endPoint: line.EndPoint, toolAngle: angle);

        /// <summary>
        /// Рез
        /// </summary>
        public void Cutting(string name, Curve curve, int feed, Point3d point, double angle)
        {
            switch (curve)
            {
                case Line _:
                    CreateCommand(name, 1, axis: "XYCZ", feed: feed, x: point.X, y: point.Y, param1: angle, param2: point.Z, toolpathCurve: curve, endPoint: point, toolAngle: angle);
                    break;
                case Arc arc:
                    var code = point == curve.StartPoint ? 2 : 3;
                    CreateCommand(name, code, axis: "XYCZ", feed: feed, x: point.X, y: point.Y, param1: arc.Center.X, param2: arc.Center.Y, toolpathCurve: curve, endPoint: point, toolAngle: angle);
                    break;
                default:
                    throw new Exception($"Неподдерживаемый тип кривой {curve.GetType()}");
            }
        }

        private void CreateCommand(string name, int gCode, int? mCode = null, string axis = null, int? feed = null, double? x = null, double? y = null, 
            double? param1 = null, double? param2 = null, Curve toolpathCurve = null, Point3d? endPoint = null, double? toolAngle = null)
            => Commands.Add(new ProcessCommand
            {
                Name = name,
                GCode = gCode.ToString(),
                MCode = mCode?.ToString(),
                Axis = axis,
                Feed = feed.ToString(),
                X = Round(x),
                Y = Round(y),
                Param1 = Round(param1),
                Param2 = Round(param2),
                ToolpathCurve = toolpathCurve,
                EndPoint = endPoint,
                ToolAngle = toolAngle
            });

        private static string Round(double? value) => value.HasValue ? Math.Round(value.Value, 4).ToString() : null;
    }
}
