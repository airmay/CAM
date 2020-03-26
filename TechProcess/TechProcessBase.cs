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

        public TechProcessBase(string caption) => Caption = caption;

        public virtual void Setup()
        {
            if (OriginX != 0 || OriginY != 0)
                OriginObject = Acad.CreateOriginObject(new Point3d(OriginX, OriginY, 0));

            TechOperations.ForEach(p => p.Setup(this));
        }

        public IEnumerable<ObjectId> ToolpathObjectIds => TechOperations.Where(p => p.ToolpathObjectIds != null).SelectMany(p => p.ToolpathObjectIds);

        public void DeleteProcessCommands()
        {
            TechOperations.ForEach(p => p.ProcessCommands = null);
            ProcessCommands = null;
        }

        public bool TechOperationMoveDown(ITechOperation techOperation) => TechOperations.SwapNext(techOperation);

        public bool TechOperationMoveUp(ITechOperation techOperation) => TechOperations.SwapPrev(techOperation);

        public virtual void BuildProcessing(int zSafety)
        {
            if (!Validate() || TechOperations.Any(p => p.Enabled && !p.Validate()))
                return;
            DeleteProcessCommands();

            using (var generator = CommandGeneratorFactory.Create(MachineType.Value))
            {
                generator.StartTechProcess(Caption, OriginX, OriginY, zSafety);
                TechOperations.FindAll(p => p.Enabled).ForEach(p =>
                {
                    generator.StartTechOperation();
                    generator.SetTool(1, Frequency);
                    p.BuildProcessing(generator);
                    p.ProcessCommands = generator.FinishTechOperation();
                });
                ProcessCommands = generator.FinishTechProcess();
            }
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