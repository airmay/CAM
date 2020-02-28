using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    /// <summary>
    /// Технологический процесс обработки
    /// </summary>
    [Serializable]
    public abstract class TechProcessBase : ITechProcess, IHasProcessCommands
    {
        [NonSerialized]
        protected Settings _settings;

        public MachineSettings MachineSettings => _settings.GetMachineSettings(MachineType);

        public string Caption { get; set; }

        public MachineType MachineType { get; set; }

        public Material Material { get; set; }

        public Tool Tool { get; set; }

        public int Frequency { get; set; }

        public ProcessingArea ProcessingArea { get; set; }

        public List<ITechOperation> TechOperations { get; } = new List<ITechOperation>();

        public List<ProcessCommand> ProcessCommands { get; set; }

        public double OriginX { get; set; }

        public double OriginY { get; set; }

        [NonSerialized]
        public ObjectId[] _originObject;

        public ObjectId[] OriginObject
        {
            get => _originObject;
            set => _originObject = value;
        }

        [NonSerialized]
        private ToolObject _toolModel;

        public ToolObject ToolObject
        {
            get => _toolModel;
            set => _toolModel = value;
        }

        public TechProcessBase(string caption, Settings settings)
        {
            Caption = caption;
            _settings = settings;
        }

        public virtual void Init(Settings settings)
        {
            _settings = settings;
            ProcessingArea.Refresh();
            if (OriginX != 0 || OriginY != 0)
                OriginObject = Acad.CreateOriginObject(new Point3d(OriginX, OriginY, 0));

            TechOperations.ForEach(to =>
            {
                //to.ProcessingArea.AcadObjectIds = Acad.GetObjectIds(to.ProcessingArea.Handles);
                to.TechProcess = this;
            });
        }

        //public abstract ITechOperation[] CreateTechOperations(string techOperationName);

        public IEnumerable<Curve> ToolpathCurves => TechOperations.Where(p => p.ToolpathCurves != null).SelectMany(p => p.ToolpathCurves);

        public void DeleteProcessCommands()
        {
            TechOperations.ForEach(p => p.ProcessCommands = null);
            ProcessCommands = null;
        }

        public bool TechOperationMoveDown(ITechOperation techOperation) => TechOperations.SwapNext(techOperation);

        public bool TechOperationMoveUp(ITechOperation techOperation) => TechOperations.SwapPrev(techOperation);

        public virtual void BuildProcessing() //BorderProcessingArea startBorder = null)
        {
            if (!Validate())
                return;
            DeleteProcessCommands();
            ProcessingArea?.Refresh();
            //BorderProcessingArea.ProcessBorders(TechOperations.Select(p => p.ProcessingArea).OfType<BorderProcessingArea>().ToList(), startBorder);
            var builder = new ScemaLogicProcessBuilder(MachineType, Caption, Tool.Number, Frequency, MachineSettings.ZSafety, OriginX, OriginY);
            TechOperations.ForEach(p => p.BuildProcessing(builder));
            ProcessCommands = builder.FinishTechProcess();
        }

        public virtual List<ITechOperation> CreateTechOperations() => new List<ITechOperation>();

        public virtual bool Validate()
        {
            if (Tool == null)
            {
                Acad.Alert("Не указан инструмент");
                return false;
            }
            return true;
        }
    }
}