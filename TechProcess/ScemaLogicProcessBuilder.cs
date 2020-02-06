using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;

namespace CAM
{
    /// <summary>
    /// Генератор команд процесса обработки
    /// </summary>
    public class ScemaLogicProcessBuilder
    {
        const int CornerIndentIncrease = 5;
        private int ZSafety;
        private int Frequency;

        private static readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>()
        {
            [CommandNames.Cutting] = Color.FromColor(System.Drawing.Color.Green),
            [CommandNames.Uplifting] = Color.FromColor(System.Drawing.Color.Blue),
            [CommandNames.Penetration] = Color.FromColor(System.Drawing.Color.Yellow),
            [CommandNames.Fast] = Color.FromColor(System.Drawing.Color.Crimson),
            [CommandNames.Transition] = Color.FromColor(System.Drawing.Color.Yellow)
        };
        //private readonly TechProcessParams _techProcessParams;
        private readonly ScemaLogicCommandGenerator _generator;
        private Point3d _currentPoint = Algorithms.NullPoint3d;
        private double _currentAngle;
        private Corner? _startCorner;

        public ScemaLogicProcessBuilder(int toolNumber, int frequency, int zSafety)
        {
            ZSafety = zSafety;
            Frequency = frequency;
            //_techProcessParams = techProcessParams;
            _generator = new ScemaLogicCommandGenerator();
            _generator.StartMachine(toolNumber);
        }

        public List<ProcessCommand> FinishTechProcess()
        {
            _generator.StopMachine();
            return _generator.Commands;
        }

        public void StartTechOperation() =>  _generator.StartRange();

        public List<ProcessCommand> FinishTechOperation()
        {
            if (_currentPoint.Z != ZSafety)
                Uplifting();
            return _generator.GetRange();
        }

        /// <summary>
        /// Поднятние
        /// </summary>
        public void Uplifting() => _generator.Uplifting(LineTo(new Point3d(_currentPoint.X, _currentPoint.Y, ZSafety), CommandNames.Uplifting), _currentAngle);

        /// <summary>
        /// Перемещение над деталью
        /// </summary>
        private void Move(Point3d point, double angle)
        {
            var destPoint = new Point3d(point.X, point.Y, ZSafety);
            if (_currentPoint.IsNull())
                _generator.InitialMove(destPoint, angle, Frequency);
            else
                _generator.Fast(LineTo(destPoint), angle);
            _currentPoint = destPoint;
            _currentAngle = angle;
        }

        /// <summary>
        /// Рез к точке
        /// </summary>
        public void Cutting(Point3d point, double angle, int cuttingFeed)
        {
            if (_currentPoint.IsNull() || _currentPoint.Z == ZSafety)
                Move(point, angle);

            _currentAngle = angle;
            CuttingCommand(CommandNames.Penetration, point, cuttingFeed);
        }

        /// <summary>
        /// Рез между точками
        /// </summary>
        public void Cutting(Point3d startPoint, Point3d endPoint, int cuttingFeed, int transitionFeed) => Cutting(NoDraw.Line(startPoint, endPoint), cuttingFeed, transitionFeed);

        /// <summary>
        /// Рез по кривой
        /// </summary>
        public void Cutting(Curve curve, int cuttingFeed, int transitionFeed)
        {
            var point = curve.StartPoint;
            if (_currentPoint.IsNull() || _currentPoint.Z == ZSafety)
            {
                if (_startCorner.HasValue)
                {
                    point = curve.GetPoint(_startCorner.Value);
                    _startCorner = null;
                }
                else if (!_currentPoint.IsNull())
                    point = (curve.StartPoint - _currentPoint).Length <= (curve.EndPoint - _currentPoint).Length ? curve.StartPoint : curve.EndPoint;

                Move(point, CalcToolAngle(curve, point));
            }
            else
            {
                point = (curve.StartPoint - _currentPoint).Length <= (curve.EndPoint - _currentPoint).Length ? curve.StartPoint : curve.EndPoint;
                _currentAngle = CalcToolAngle(curve, point);
            }
            CuttingCommand(point.Z !=_currentPoint.Z ? CommandNames.Penetration : CommandNames.Transition, point, transitionFeed);
            _currentPoint = curve.NextPoint(point);
            _currentAngle = CalcToolAngle(curve, _currentPoint);
            _generator.Cutting(CommandNames.Cutting, curve, cuttingFeed, _currentPoint, _currentAngle);
            curve.Color = Colors[CommandNames.Cutting];
        }

        /// <summary>
        /// Команда реза
        /// </summary>
        private void CuttingCommand(string command, Point3d point, int feed) => _generator.Cutting(command, LineTo(point, command), feed, point, _currentAngle);

        private Line LineTo(Point3d point, string command = CommandNames.Fast)
        {
            var line = new Line(_currentPoint, point) { Color = Colors[command] };
            _currentPoint = point;
            return line;
        }

        /// <summary>
        /// Построчный рез
        /// </summary>
        public void LineCut(Curve curve, List<CuttingMode> modes, bool isZeroPass, bool isExactlyBegin, bool isExactlyEnd, Side toolSide, int depthAll, double compensation)
        {
            var passList = GetPassList(modes, depthAll, isZeroPass);
            _startCorner = curve.IsUpward() ^ (passList.Count() % 2 == 1) ? Corner.End : Corner.Start;
            foreach (var item in passList)
            {
                var toolpathCurve = CreateToolpath(curve, compensation, item.Key, isExactlyBegin, isExactlyEnd);
                //Cutting(toolpathCurve, item.Value, _techProcessParams.PenetrationRate);
            }
        }

        public Curve CreateToolpath(Curve curve, double offset, double depth, bool isExactlyBegin, bool isExactlyEnd)
        {
            var toolpathCurve = curve.GetOffsetCurves(offset)[0] as Curve;
            if (depth != 0)
                toolpathCurve.TransformBy(Matrix3d.Displacement(-Vector3d.ZAxis * depth));

            var indent = CalcIndent(depth);
            switch (toolpathCurve)
            {
                case Line line:
                    if (isExactlyBegin)
                        line.StartPoint = line.GetPointAtDist(indent);
                    if (isExactlyEnd)
                        line.EndPoint = line.GetPointAtDist(line.Length - indent);
                    break;

                case Arc arc:
                    var indentAngle = indent / ((Arc)curve).Radius;
                    if (isExactlyBegin)
                        arc.StartAngle = arc.StartAngle + indentAngle;
                    if (isExactlyEnd)
                        arc.EndAngle = arc.EndAngle - indentAngle;
                    var deltaStart = arc.StartPoint.X - arc.Center.X;
                    var deltaEnd = arc.EndPoint.X - arc.Center.X;
                    // if ((arc.StartAngle >= 0.5 * Math.PI && arc.StartAngle < 1.5 * Math.PI) ^ (arc.EndAngle > 0.5 * Math.PI && arc.EndAngle <= 1.5 * Math.PI))
                    if ((Math.Abs(deltaStart) > Consts.Epsilon && Math.Abs(deltaEnd) > Consts.Epsilon && (deltaStart > 0 ^ deltaEnd > 0)) || (arc.TotalAngle > Math.PI + Consts.Epsilon))
                        throw new InvalidOperationException(
                            $"Обработка дуги невозможна - дуга пересекает угол 90 или 270 градусов. Текущие углы: начальный {Graph.ToDeg(arc.StartAngle)}, конечный {Graph.ToDeg(arc.EndAngle)}");
                    break;
            }
            return toolpathCurve;
        }

        private static List<KeyValuePair<double, int>> GetPassList(IEnumerable<CuttingMode> modes, double DepthAll, bool isZeroPass)
        {
            var passList = new List<KeyValuePair<double, int>>();
            var enumerator = modes.OrderBy(p => p.Depth).GetEnumerator();
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Не заданы режимы обработки");
            var mode = enumerator.Current;
            CuttingMode nextMode = null;
            if (enumerator.MoveNext())
                nextMode = enumerator.Current;
            var depth = isZeroPass ? -mode.DepthStep : 0;
            do
            {
                depth += mode.DepthStep;
                if (nextMode != null && depth >= nextMode.Depth)
                {
                    mode = nextMode;
                    nextMode = enumerator.MoveNext() ? enumerator.Current : null;
                }
                if (depth > DepthAll)
                    depth = DepthAll;
                passList.Add(new KeyValuePair<double, int>(depth, mode.Feed));
            }
            while (depth < DepthAll);

            return passList;
        }

        public double CalcToolAngle(Curve curve, Point3d point)
        {
            var tangent = curve.GetFirstDerivative(point).ToVector2d();
            if (!curve.IsUpward())
                tangent = tangent.Negate();
            var angle = Graph.ToDeg(Math.PI - Graph.Round(tangent.Angle));
            if (curve is Line && angle == 180)
                angle = 0;
            return angle;
        }

        public double CalcCompensation(Curve curve, Side toolSide)
        {
            return 0;
            //var depth = _techProcessParams.BilletThickness;
            //var offset = 0d;
            //if (curve.IsUpward() ^ toolSide == Side.Left)
            //    offset = _techProcessParams.ToolThickness;
            //if (curve is Arc arc && toolSide == Side.Left)
            //    offset += arc.Radius - Math.Sqrt(arc.Radius * arc.Radius - depth * (_techProcessParams.ToolDiameter - depth));
            //return toolSide == Side.Left ^ curve is Arc ? offset : -offset;
        }

        public double CalcIndent(double depth) => 0; // Math.Sqrt(depth * (_techProcessParams.ToolDiameter - depth)) + CornerIndentIncrease;

    }
}
