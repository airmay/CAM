using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Генератор команд процесса обработки
    /// </summary>
    public class ProcessBuilder
    {
        //private static Dictionary<string, string> CommandCodes =
        //{
        //    [
        //}

        private Point3d _currentPoint = Point3d.Origin;
        private int _currentFeed;
        private TechProcessParams _techProcessParams;

        public List<Curve> Entities { get; } = new List<Curve>();

        public List<ProcessCommand> Commands { get; } = new List<ProcessCommand>();

        public ProcessBuilder(TechProcessParams techProcessParams)
        {
            _techProcessParams = techProcessParams;
        }

        private void Move(string name, string code, double? x = null, double? y = null, double? z = null, int? feed = null)
        {
            var destPoint = new Point3d(x ?? _currentPoint.X, y ?? _currentPoint.Y, z ?? _currentPoint.Z);
            // TODO проверка совпадения точек
            var line = new Line(_currentPoint, destPoint);
            line.ObjectId = new ObjectId(name);
            CreateCommand(name, code, line, "XYZ", feed, destPoint.X, destPoint.Y, destPoint.Z);
            _currentPoint = destPoint;
            Entities.Add(line);
        }

        internal void Setup(Curve curve)
        {
            throw new NotImplementedException();
        }

        public void SetTool(double x, double y, double c)
        {
            string name = _currentPoint == Point3d.Origin ? CommandNames.InitialMove : CommandNames.Fast;
            Line line = null;
            var z = _techProcessParams.ZSafety;
            var destPoint = new Point3d(x, y, z);
            if (_currentPoint != Point3d.Origin)
            CreateCommand(name, "0", line, "XYC", 0, x, y, c);
            CreateCommand(name, "0", line, "XYZ", 0, x, y, z);
            if (_currentPoint == Point3d.Origin)
            {
                name = "Setup";
                CreateCommand(name, "97;7");
                CreateCommand(name, "97;8");
                CreateCommand(name, "97;3", feed: _techProcessParams.Frequency);
            }
            else
            {
                line = new Line(_currentPoint, destPoint);
                line.ObjectId = new ObjectId(name);
                Entities.Add(line);
            }
            CreateCommand("Setup", "28", plane: "XYCZ");  // цикл
            _currentPoint = destPoint;
        }

        private void CreateCommand(string name, string code, Curve toolpathAcadObject= null, string plane = null, int? feed = null, double? param3 = null, double? param4 = null, double? param5 = null, double? param6 = null) 
            => Commands.Add(new ProcessCommand(name, code, toolpathAcadObject, plane, feed?.ToString(), GetParam(param3), GetParam(param4), GetParam(param5), GetParam(param6)));

        private string GetParam(double? value, string paramName = null) => value.HasValue ? $" {paramName}{value:0.####}" : null;

        /// <summary>
        /// Опускание
        /// </summary>
        /// <param name="z"></param>
        //public void Descent(double z) => MoveZ(CommandNames.Descent, z, _techProcessParams.PenetrationRate);

        /// <summary>
        /// Поднятие
        /// </summary>
        public void Uplifting() => MoveZ(CommandNames.Uplifting, "0", _techProcessParams.ZSafety, 0);

        /// <summary>
        /// Заглубление
        /// </summary>
        /// <param name="z"></param>
        public void Penetration(double z) => MoveZ(CommandNames.Penetration, "1", z, _techProcessParams.PenetrationRate);

        private void MoveZ(string name, string code, double z, int feed) => Move(name, code, z: z, feed: feed);

        /// <summary>
        /// Рабочий ход
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="dz"></param>
        /// <param name="offset"></param>
        /// <param name="startIndent"></param>
        /// <param name="endIndent"></param>
        /// <param name="feed"></param>
        public void Cutting(Curve curve, double dz = 0, double offset = 0, double startIndent = 0, double endIndent = 0, int feed = 0)
        {
            var toolpathCurve = curve.GetOffsetCurves(offset, dz);
            toolpathCurve.ObjectId = new ObjectId($"{curve.ObjectId.Key} обработка");
            if (toolpathCurve.StartPoint == _currentPoint)
                _currentPoint = toolpathCurve.EndPoint;
            else if (toolpathCurve.EndPoint == _currentPoint)
                _currentPoint = toolpathCurve.StartPoint;
            else
                throw new Exception("Несоответствие текущей позиции и рассчитанной траектории");

            switch (toolpathCurve)
            {
                case Line line:
                    var c = ToDeg((Math.PI * 2 - line.Angle) % Math.PI);
                    CreateCommand(CommandNames.Cutting, "1", toolpathCurve, "XYCZ", feed, _currentPoint.X, _currentPoint.Y, c, _currentPoint.Z);
                    break;
                case Arc arc:
                    var code = toolpathCurve.StartPoint == _currentPoint ? "3" : "2";

                    bool isLeftArc = arc.StartAngle >= Math.PI * 0.5 && arc.StartAngle < Math.PI * 1.5;
                    var angle = toolpathCurve.StartPoint == _currentPoint ? arc.StartAngle : arc.EndAngle;
                    c = ToDeg(isLeftArc ? Math.PI * 1.5 - angle : (Math.PI * 2.5 - angle) % (2 * Math.PI));
                    CreateCommand(CommandNames.Cutting, code, toolpathCurve, "XYCZ", feed, _currentPoint.X, _currentPoint.Y, arc.Center.X, arc.Center.Y);

                    break;
                default:
                    throw new Exception($"Неподдерживаемый тип кривой {toolpathCurve.GetType()}");
            }
            Entities.Add(toolpathCurve);
        }

        private double ToDeg(double angle) => angle * 180 / Math.PI;
    }


    ///// <summary>
    ///// Генератор команд процесса обработки
    ///// </summary>
    //public class ProcessBuilder
    //{
    //    //private static Dictionary<string, string> CommandCodes =
    //    //{
    //    //    [
    //    //}

    //    private Point3d _currentPoint = Point3d.Origin;
    //    private int _currentFeed;
    //    private TechProcessParams _techProcessParams;

    //    public List<Curve> Entities { get; } = new List<Curve>();

    //    public List<ProcessCommand> Commands { get; } = new List<ProcessCommand>();

    //    public ProcessBuilder(TechProcessParams techProcessParams)
    //    {
    //        _techProcessParams = techProcessParams;
    //    }

    //    private void Move(string name = null, string code, double? x = null, double? y = null, double? z = null, double? c = null, int? feed = null)
    //    {
    //        var destPoint = new Point3d(x ?? _currentPoint.X, y ?? _currentPoint.Y, z ?? _currentPoint.Z);
    //        // TODO проверка совпадения точек
    //        var line = new Line(_currentPoint, destPoint);
    //        line.ObjectId = new ObjectId("перемещение");
    //        Commands.Add(new ProcessCommand(name, code, line, "XYCZ", GetParam(x), GetParam(y), GetParam(c), GetParam(z)));
    //        _currentPoint = destPoint;
    //        Entities.Add(line);
    //    }

    //    private void Set(string name = null, double? x = null, double? y = null, double? z = null, double? c = null, int? feed = null)
    //    {
    //        var destPoint = new Point3d(x ?? _currentPoint.X, y ?? _currentPoint.Y, z ?? _currentPoint.Z);
    //        // TODO проверка совпадения точек
    //        var line = new Line(_currentPoint, destPoint);
    //        line.ObjectId = new ObjectId("перемещение");
    //        Commands.Add(new ProcessCommand(name, "0", line, "XYC", "0", GetParam(x), GetParam(y), GetParam(c)));
    //        Commands.Add(new ProcessCommand(name, "0", line, "XYZ", "0", GetParam(x), GetParam(y), GetParam(z)));
    //        _currentPoint = destPoint;
    //        _currentFeed = feed ?? _currentFeed;
    //        Entities.Add(line);
    //    }

    //    //private void Move(string name = null, double? x = null, double? y = null, double? z = null, int? feed = null)
    //    //{
    //    //    var destPoint = new Point3d(x ?? _currentPoint.X, y ?? _currentPoint.Y, z ?? _currentPoint.Z);
    //    //    // TODO проверка совпадения точек
    //    //    var line = new Line(_currentPoint, destPoint);
    //    //    line.ObjectId = new ObjectId("перемещение");
    //    //    //var par = new List<string>();
    //    //    //if (x.HasValue) par.Add($"X{x:0.####}");
    //    //    //if (y.HasValue) par.Add($"Y{y:0.####}");
    //    //    //if (z.HasValue) par.Add($"Z{z:0.####}");
    //    //    //if (feed.HasValue) par.Add($"F{feed:0.####}");
    //    //    //            Commands.Add(new ProcessCommand(name, "G1", line, par.ToArray()));
    //    //    //            Commands.Add(new ProcessCommand(name, CommandCode.G1, line, GetParam("X", x), GetParam("Y", y), GetParam("Z", z), GetParam("F", feed)));
    //    //    Commands.Add(new ProcessCommand(name, CommandCode.G1, line, GetParam("X", x), GetParam("Y", y), GetParam("Z", z), GetParam("F", feed)));
    //    //    Commands.Add(new ProcessCommand("XYC", point[vertex.Index()].X, point[vertex.Index()].Y, angle[vertex.Index()], obj.ToString());
    //    //    Commands.Add(new ProcessCommand("XYZ", point[vertex.Index()].X, point[vertex.Index()].Y, ProcessOptions.ZSafety, obj.ToString());
    //    //    _currentPoint = destPoint;
    //    //    _currentFeed = feed ?? _currentFeed;
    //    //    Entities.Add(line);
    //    //}

    //    private string GetParam(double? value, string paramName = null) => value.HasValue ? $" {paramName}{value:0.####}" : null;

    //    //private void MoveZ(string name, double z, int feed = 0) => Move(name: name, z: z, feed: feed);

    //    /// <summary>
    //    /// Быстрая подача
    //    /// </summary>
    //    /// <param name="x"></param>
    //    /// <param name="y"></param>
    //    public void Move(double x, double y)
    //    {
    //        if (_currentPoint == Point3d.Origin)
    //        {
    //            _currentPoint = new Point3d(x, y, _techProcessParams.ZSafety);
    //            Move(CommandNames.InitialMove, x, y));
    //        }
    //        else
    //            MoveXYZ(x, y, _currentPoint.Z, CommandNames.Fast);
    //    }

    //    /// <summary>
    //    /// Опускание
    //    /// </summary>
    //    /// <param name="z"></param>
    //    public void Descent(double z) => MoveZ(CommandNames.Descent, z);

    //    /// <summary>
    //    /// Поднятие
    //    /// </summary>
    //    public void Uplifting() => MoveZ(CommandNames.Uplifting, _techProcessParams.ZSafety);

    //    /// <summary>
    //    /// Заглубление
    //    /// </summary>
    //    /// <param name="z"></param>
    //    public void Penetration(double z) => MoveZ(CommandNames.Penetration, z, _techProcessParams.PenetrationRate);

    //    /// <summary>
    //    /// Рабочий ход
    //    /// </summary>
    //    /// <param name="curve"></param>
    //    /// <param name="dz"></param>
    //    /// <param name="offset"></param>
    //    /// <param name="startIndent"></param>
    //    /// <param name="endIndent"></param>
    //    /// <param name="feed"></param>
    //    public void Cutting(Curve curve, double dz = 0, double offset = 0, double startIndent = 0, double endIndent = 0, int feed = 0)
    //    {
    //        var toolpathCurve = curve.GetOffsetCurves(dz);
    //        toolpathCurve.ObjectId = new ObjectId($"{curve.ObjectId.Key} обработка");
    //        if (toolpathCurve.StartPoint == _currentPoint)
    //            _currentPoint = toolpathCurve.EndPoint;
    //        else if (toolpathCurve.EndPoint == _currentPoint)
    //            _currentPoint = toolpathCurve.StartPoint;
    //        else
    //            throw new Exception("Несоответствие текущей позиции и рассчитанной траектории");

    //        Commands.Add(new ProcessCommand(CommandNames.Cutting, CommandCode.G1, toolpathCurve));
    //        _currentPoint = destPoint;
    //        _currentFeed = feed ?? _currentFeed;
    //        Entities.Add(line);

    //        ProcessActions.Add(new ProcessAction($"{CommandNames.Cutting} F{feed}", _groupName, toolpathCurve, feed));
    //        Entities.Add(toolpathCurve);
    //    }
    //}

}
