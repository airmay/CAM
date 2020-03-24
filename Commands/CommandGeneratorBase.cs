using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System.Collections.Generic;

namespace CAM
{
    public abstract class CommandGeneratorBase : ICommandGenerator
    {
        protected Location _location;
        protected int _toolIndex;
        protected int _GCode;
        protected int _feed;
        protected double _originX, _originY;
        protected int _frequency;

        private const int UpperZ = 80;
        private List<ProcessCommand> _commands = new List<ProcessCommand>();
        private int _startRangeIndex;
        private int _zSafety;
        private bool _isEngineStarted = false;

        private DocumentLock _documentLock;
        private Transaction _transaction;
        private BlockTableRecord _currentSpace;
        private ObjectId _layerId = Acad.GetProcessLayerId();
        private readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>()
        {
            [CommandNames.Cutting] = Color.FromColor(System.Drawing.Color.Green),
            [CommandNames.Uplifting] = Color.FromColor(System.Drawing.Color.Blue),
            [CommandNames.Penetration] = Color.FromColor(System.Drawing.Color.Yellow),
            [CommandNames.Fast] = Color.FromColor(System.Drawing.Color.Crimson),
            [CommandNames.Transition] = Color.FromColor(System.Drawing.Color.Yellow)
        };

        public bool IsUpperTool => _location.Point.Z >= _zSafety;
        public bool WithThick { get; set; }

        public void StartTechProcess(string caption, double originX, double originY, int zSafety)
        {
            _originX = originX;
            _originY = originY;
            _zSafety = zSafety;

            _documentLock = Acad.ActiveDocument.LockDocument();
            _transaction = Acad.Database.TransactionManager.StartTransaction();
            _currentSpace = (BlockTableRecord)_transaction.GetObject(Acad.Database.CurrentSpaceId, OpenMode.ForWrite, false);

            StartMachineCommands(caption);
        }

        /// <summary>
        /// Запуск станка
        /// </summary>
        protected abstract void StartMachineCommands(string caption);

        public List<ProcessCommand> FinishTechProcess()
        {
            StopEngine();
            StopMachineCommands();

            _transaction.Commit();

            int number = 0;
            _commands.ForEach(p => p.Number = ++number);

            return _commands;
        }

        public void Dispose()
        {
            _transaction.Dispose();
            _documentLock.Dispose();
        }

        /// <summary>
        /// Остановка станка
        /// </summary>
        protected abstract void StopMachineCommands();

        public void StartTechOperation() => _startRangeIndex = _commands.Count;

        public List<ProcessCommand> FinishTechOperation()
        {
            if (!IsUpperTool)
                Uplifting();
            return _commands.GetRange(_startRangeIndex, _commands.Count - _startRangeIndex);
        }

        public void SetTool(int toolNo, int frequency, double angleA = 0)
        {
            if (_toolIndex != toolNo)
            {
                StopEngine();
                SetToolCommands(toolNo, angleA);

                _toolIndex = toolNo;
                _location.Set(new Point3d(double.NaN, double.NaN, UpperZ), 0, 0);
            }
            _frequency = frequency;
        }

        protected abstract void SetToolCommands(int toolNo, double angleA);

        private void StopEngine()
        {
            if (_isEngineStarted)
                StopEngineCommands();
            _isEngineStarted = false;
        }

        protected abstract void StopEngineCommands();

        /// <summary>
        /// Поднятние
        /// </summary>
        public void Uplifting(double? z = null) => GCommand(CommandNames.Uplifting, 0, z: z ?? _zSafety);

        public void Uplifting(Vector3d vector) => GCommand(CommandNames.Uplifting, 0, point: _location.Point + vector);

        /// <summary>
        /// Подвод
        /// </summary>
        public void Move(double x, double y, double angleC, double angleA = 0) => Move(new Point3d(x, y, _zSafety), angleC, angleA);

        public void Move(Point3d point, double angleC, double angleA = 0)
        {
            GCommand(_isEngineStarted ? CommandNames.Fast : CommandNames.InitialMove, 0, point: point, angleC: angleC);
            if (!_isEngineStarted)
                GCommand(CommandNames.InitialMove, 0, z: _zSafety);

            if (angleA != _location.AngleA)
                GCommand("Наклон", 1, angleA: angleA);

            if (!_isEngineStarted)
            {
                StartEngineCommands();
                _isEngineStarted = true;
            }
        }

        protected abstract void StartEngineCommands();

        /// <summary>
        /// Рез к точке
        /// </summary>
        public void Cutting(double x, double y, double z, double angleC, int feed, double angleA = 0)
        {
            if (IsUpperTool)
                Move(x, y, angleC, angleA);
            GCommand(CommandNames.Penetration, 1, x: x, y: y, z: z, angleC: angleC, angleA: angleA, feed: feed);
        }

        /// <summary>
        /// Рез между точками
        /// </summary>
        public void Cutting(Point3d startPoint, Point3d endPoint, int cuttingFeed, int transitionFeed) => Cutting(NoDraw.Line(startPoint, endPoint), cuttingFeed, transitionFeed);

        /// <summary>
        /// Рез по кривой
        /// </summary>
        public void Cutting(Curve curve, int cuttingFeed, int transitionFeed, Side engineSide = Side.None)
        {
            var point = curve.GetClosestPoint(_location.Point);
            var angleC = _location.AngleC;
            if (curve is Arc)
                angleC = BuilderUtils.CalcToolAngle(curve, point, engineSide);
            GCommand(point.Z != _location.Point.Z ? CommandNames.Penetration : CommandNames.Transition, 1, point: point, angleC: angleC, feed: transitionFeed);

            if (curve is Arc arc)
                angleC += arc.TotalAngle.ToDeg() * (point == curve.StartPoint ? -1 : 1);
            GCommand(CommandNames.Cutting, 1, point: curve.NextPoint(point), angleC: angleC, curve: curve, feed: cuttingFeed);
        }

        public void Command(string text, string name = null) => _commands.Add(new ProcessCommand
        {
            Name = name,
            Text = text,
            ToolIndex = _toolIndex,
            ToolLocation = _location
        });

        public void GCommand(string name, int gCode, string paramsString = null, Point3d? point = null, double? x = null, double? y = null, double? z = null,
            double? angleC = null, double? angleA = null, Curve curve = null, int? feed = null)
        {
            if (point == null)
                point = new Point3d(x ?? _location.Point.X, y ?? _location.Point.Y, z ?? _location.Point.Z);

            var commandText = GCommandText(gCode, paramsString, point.Value, curve, angleC, angleA, feed);

            var toolpathObjectId = ObjectId.Null;
            if (_location.IsDefined)
            {
                if (curve == null && (point.Value - _location.Point).Length > 1)
                    curve = NoDraw.Line(_location.Point, point.Value);

                if (curve != null)
                {
                    if (Colors.ContainsKey(name))
                        curve.Color = Colors[name];
                    curve.LayerId = _layerId;
                    toolpathObjectId = _currentSpace.AppendEntity(curve);
                    _transaction.AddNewlyCreatedDBObject(curve, true);
                }
            }

            _GCode = gCode;
            _location.Set(point.Value, angleC, angleA);
            _feed = feed ?? _feed;

            _commands.Add(new ProcessCommand
            {
                Name = name,
                Text = commandText,
                ToolpathObjectId = toolpathObjectId,
                ToolIndex = _toolIndex,
                ToolLocation = _location
            });
        }

        protected abstract string GCommandText(int gCode, string paramsString, Point3d point, Curve curve, double? angleC, double? angleA, int? feed);
    }
}
