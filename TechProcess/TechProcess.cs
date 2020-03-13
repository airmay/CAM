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

        public MachineType? MachineType { get; set; }

        public Material? Material { get; set; }

        public Tool Tool { get; set; }

        public int Frequency { get; set; }

        public AcadObjectGroup ProcessingArea { get; set; }

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

        public TechProcessBase(string caption, Settings settings)
        {
            Caption = caption;
            _settings = settings;
        }

        public virtual void Setup(Settings settings)
        {
            _settings = settings;
            if (OriginX != 0 || OriginY != 0)
                OriginObject = Acad.CreateOriginObject(new Point3d(OriginX, OriginY, 0));

            TechOperations.ForEach(to =>
            {
                to.Setup(this);
                //to.ProcessingArea.AcadObjectIds = Acad.GetObjectIds(to.ProcessingArea.Handles);
            });
        }

        //public abstract ITechOperation[] CreateTechOperations(string techOperationName);

        public IEnumerable<ObjectId> ToolpathObjectIds => TechOperations.Where(p => p.ToolpathObjectIds != null).SelectMany(p => p.ToolpathObjectIds);

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
            
            //BorderProcessingArea.ProcessBorders(TechOperations.Select(p => p.ProcessingArea).OfType<BorderProcessingArea>().ToList(), startBorder);

            var builder = new ScemaLogicProcessBuilder(MachineType.Value, Caption, OriginX, OriginY, MachineSettings.ZSafety);
            TechOperations.ForEach(p =>
            {
                builder.StartTechOperation();
                builder.SetTool(1, Frequency);
                p.BuildProcessing(builder);
                p.ProcessCommands = builder.FinishTechOperation();
            });
            ProcessCommands = builder.FinishTechProcess();
        }

        public virtual List<ITechOperation> CreateTechOperations() => new List<ITechOperation>();

        public virtual bool Validate() => Tool.CheckNotNull("Инструмент");

        public virtual void Teardown()
        {
            Acad.DeleteObjects(OriginObject);
            TechOperations.ForEach(to => to.Teardown());
        }
    }
}