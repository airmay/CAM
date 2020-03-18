using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
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
        private int ZSafety;

        private static readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>()
        {
            [CommandNames.Cutting] = Color.FromColor(System.Drawing.Color.Green),
            [CommandNames.Uplifting] = Color.FromColor(System.Drawing.Color.Blue),
            [CommandNames.Penetration] = Color.FromColor(System.Drawing.Color.Yellow),
            [CommandNames.Fast] = Color.FromColor(System.Drawing.Color.Crimson),
            [CommandNames.Transition] = Color.FromColor(System.Drawing.Color.Yellow)
        };

        //private readonly TechProcessParams _techProcessParams;
        //private readonly ScemaLogicCommandGenerator _generator = new ScemaLogicCommandGenerator();
        private readonly DonatoniCommandGeneratorOld _generator = new DonatoniCommandGeneratorOld();
        //private Point3d _currentPoint = Algorithms.NullPoint3d;
        //private double _currentAngle;

        private DocumentLock _documentLock;
        private Transaction _transaction;
        private BlockTableRecord _currentSpace;

        public ScemaLogicProcessBuilder(MachineType machineType, string caption, double originX, double originY, int zSafety)
        {
            _documentLock = Acad.ActiveDocument.LockDocument();
            _transaction = Acad.Database.TransactionManager.StartTransaction();
            _currentSpace = (BlockTableRecord)_transaction.GetObject(Acad.Database.CurrentSpaceId, OpenMode.ForWrite, false);
            _generator.Transaction = _transaction;
            _generator.CurrentSpace = _currentSpace;
                ZSafety = zSafety;
            //Frequency = frequency;
            //_techProcessParams = techProcessParams;
            //_generator = new DonatoniCommandGenerator();
            //_generator = new DonatoniCommandGenerator();
            _generator.StartMachine(caption, originX, originY, zSafety);
        }

        public List<ProcessCommand> FinishTechProcess()
        {
            _generator.StopMachine();
            _transaction.Commit();
            _transaction.Dispose();
            _documentLock.Dispose();
            return _generator.Commands;
        }

        public void StartTechOperation() => _generator.StartRange();

        public void SetTool(int toolNo, int frequency, double angleA = 0) => _generator.SetTool(toolNo, frequency, angleA);

        public List<ProcessCommand> FinishTechOperation()
        {
            if (!_generator.IsUpperTool)
                Uplifting();
            return _generator.GetRange();
        }

        /// <summary>
        /// Поднятние
        /// </summary>
        //public void Uplifting() => _generator.Uplifting(LineTo(new Point3d(_currentPoint.X, _currentPoint.Y, ZSafety), CommandNames.Uplifting), _currentAngle);
        public void Uplifting(double? z = null) => _generator.CreateCommand(CommandNames.Uplifting, 0, z: z ?? ZSafety);

        /// <summary>
        /// Перемещение над деталью
        /// </summary>
        //private void Move(double x, double y, double angleC, double angleA = 0)
        //{
        //    //var destPoint = new Point3d(point.X, point.Y, ZSafety);
        //    if (_generator.EngineStarted)
        //        _generator.CreateCommand(CommandNames.Fast, 0, x: x, y: y, angleC: angleC);
        //    else
        //        _generator.InitialMove(x, y, ZSafety, angleC, Frequency);
        //    if (angleA != _generator.ToolInfo.AngleA)
        //        _generator.CreateCommand("", 1, angleA: angleA);

        //}

        /// <summary>
        /// Рез к точке
        /// </summary>
        public void Cutting(double x, double y, double z, double angleC, int cuttingFeed, double angleA = 0)
        {
            if (_generator.IsUpperTool)
                _generator.Move(x, y, angleC, angleA);
            _generator.CreateCommand(CommandNames.Penetration, 1, x: x, y: y, z: z, angleC: angleC, angleA: angleA, feed: cuttingFeed);
        }

        /// <summary>
        /// Рез между точками
        /// </summary>
        public void Cutting(Point3d startPoint, Point3d endPoint, int cuttingFeed, int transitionFeed, double angleA = 0, Side engineSide = Side.Right) 
            => Cutting(NoDraw.Line(startPoint, endPoint), cuttingFeed, transitionFeed, angleA, engineSide);

        /// <summary>
        /// Рез по кривой
        /// </summary>
        public void Cutting(Curve curve, int cuttingFeed, int transitionFeed, double angleA = 0, Side engineSide = Side.Right, Corner? corner = null)
        {
            var point = corner.HasValue ? curve.GetPoint(corner.Value) : curve.GetClosestPoint(_generator.ToolPosition);
            var angleC = CalcToolAngle(curve, point, engineSide);
            if (_generator.IsUpperTool)
                _generator.Move(point.X, point.Y, angleC, angleA);

            _generator.CreateCommand(point.Z != _generator.ToolPosition.Z ? CommandNames.Penetration : CommandNames.Transition, 1, point: point, angleC: angleC, angleA: angleA, feed: transitionFeed);
            point = curve.NextPoint(point);
            if (!(curve is Line))
                angleC = CalcToolAngle(curve, point, engineSide);
            _generator.CreateCommand(CommandNames.Cutting, 1, point: point, angleC: angleC, curve: curve, feed: cuttingFeed);
        }

        public void Pause()
        {
            _generator.CreateCommand("G4 F0.2", "Пауза");
        }

        public void Measure(List<double> pointsX, List<double> pointsY)
        {
            for (int i = 0; i < pointsX.Count; i++)
            {
                _generator.CreateCommand(CommandNames.Fast, 0, paramsString: "XY", x: pointsX[i], y: pointsY[i], z: DonatoniCommandGeneratorOld.UpperZ);
                _generator.CreateCommand($"G0 Z{DonatoniCommandGeneratorOld.UpperZ}");
                _generator.CreateCommand("M131");
                _generator.CreateCommand($"DBL THICK{i} = %TastL.ZLastra - %TastL.ZBanco", "Измерение");
                _generator.CreateCommand($"G0 Z(THICK{i}/1000 + 100)");
            }
            var s = String.Join(" + ", Enumerable.Range(0, pointsX.Count).Select(p => $"THICK{p}"));
            _generator.CreateCommand($"DBL THICK = ({s})/{pointsX.Count}/1000");
            _generator.WithThick = true;
        }

        public static IEnumerable<KeyValuePair<double, int>> GetPassList(IEnumerable<CuttingMode> modes, double DepthAll, bool isZeroPass)
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
                yield return new KeyValuePair<double, int>(depth, mode.Feed);
            }
            while (depth < DepthAll);
        }

        public static double CalcToolAngle(Curve curve, Point3d point, Side engineSide)
        {
            var tangent = curve.GetFirstDerivative(point).ToVector2d();
            if (!curve.IsUpward())
                tangent = tangent.Negate();
            var angle = Graph.ToDeg(Math.PI - tangent.Angle.Round(6));
            if (curve is Line && angle == 180)
                angle = 0;
            if (engineSide == Side.Left)
                angle += 180;
            return angle;
        }
    }
}
