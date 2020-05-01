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

        public double? Thickness { get; set; }

        public Tool Tool { get; set; }

        public int Frequency { get; set; }

        public int PenetrationFeed { get; set; }

        public double ZSafety { get; set; }

        public List<AcadObject> ProcessingArea { get; set; }

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

        public TechProcessBase(string caption)
        {
            Caption = caption;
            ZSafety = 20;
        }

        public virtual void Setup()
        {
            if (OriginX != 0 || OriginY != 0)
                OriginObject = Acad.CreateOriginObject(new Point3d(OriginX, OriginY, 0));

            TechOperations.ForEach(p => p.Setup(this));
        }

        public IEnumerable<ObjectId> ToolpathObjectIds => ProcessCommands?.Where(p => p.ToolpathObjectId != null).Select(p => p.ToolpathObjectId.Value);

        public void SetToolpathVisible(bool visible) => ToolpathObjectIds?.ForEach<Curve>(p => p.Visible = visible);

        public void DeleteProcessCommands()
        {
            TechOperations.ForEach(p => p.ProcessCommands = null);
            ProcessCommands = null;
        }

        public bool TechOperationMoveDown(ITechOperation techOperation) => TechOperations.SwapNext(techOperation);

        public bool TechOperationMoveUp(ITechOperation techOperation) => TechOperations.SwapPrev(techOperation);

        public virtual void BuildProcessing()
        {
            if (!TechOperations.Any())
                CreateTechOperations();

            if (!Validate() || TechOperations.Any(p => p.Enabled && p.CanProcess && !p.Validate()))
                return;
            DeleteProcessCommands();

            using (var generator = CommandGeneratorFactory.Create(MachineType.Value))
            {
                generator.StartTechProcess(this.GetType().Name, OriginX, OriginY, ZSafety);

                if (Tool != null)
                    generator.SetTool(
                        MachineType.Value != CAM.MachineType.Donatoni ? Tool.Number : 1,
                        Frequency);

                BuildProcessing(generator);

                TechOperations.FindAll(p => p.Enabled && p.CanProcess).ForEach(p =>
                {
                    generator.StartTechOperation();

                    p.BuildProcessing(generator);

                    if (!generator.IsUpperTool)
                        generator.Uplifting();

                    p.ProcessCommands = generator.FinishTechOperation();                    
                });
                ProcessCommands = generator.FinishTechProcess();
            }
            UpdateCaptions();
        }

        protected virtual void BuildProcessing(ICommandGenerator generator) { }

        public virtual void SkipProcessing(ProcessCommand processCommand)
        {
            List<ProcessCommand> techOperationCommands;
            List<ProcessCommand> techProcessCommands;
            using (var generator = CommandGeneratorFactory.Create(MachineType.Value))
            {
                generator.StartTechProcess(Caption, OriginX, OriginY, ZSafety);
                generator.StartTechOperation();
                generator.SetTool(
                    MachineType.Value != CAM.MachineType.Donatoni ? Tool.Number : processCommand.HasTool ? 1 : 2, 
                    Frequency);
                var loc = ProcessCommands[ProcessCommands.IndexOf(processCommand) - 1].ToolLocation;
                generator.Move(loc.Point.X, loc.Point.Y, angleC: loc.AngleC, angleA: loc.AngleA);
                generator.GCommand(CommandNames.Penetration, 1, point: loc.Point, feed: PenetrationFeed);
                techOperationCommands = generator.FinishTechOperation();
                techProcessCommands = generator.FinishTechProcess();
            }
            foreach (var techOperation in TechOperations)
            {
                if (techOperation.ProcessCommands?.Last().Number >= processCommand.Number)
                {
                    techOperation.ProcessCommands = techOperationCommands.Concat(techOperation.ProcessCommands.SkipWhile(p => p.Number < processCommand.Number)).ToList();
                    break;
                }
                else
                    techOperation.ProcessCommands = null;
            }
            ProcessCommands = techProcessCommands.TakeWhile(p => !techOperationCommands.Contains(p)).Concat(techOperationCommands)
                .Concat(ProcessCommands.SkipWhile(p => p.Number < processCommand.Number)).ToList();
            UpdateCaptions();
        }

        private void UpdateCaptions()
        {
            TechOperations.ForEach(p => p.Caption = GetCaption(p, p.Caption));
            Caption = GetCaption(this, Caption);

            string GetCaption(IHasProcessCommands obj, string text)
            {
                var duration = obj.ProcessCommands?.Sum(p => p.Duration) ?? 0;
                var ind = text.IndexOf('(');
                return $"{(ind > 0 ? text.Substring(0, ind).Trim() : text)} ({new TimeSpan(0, 0, 0, (int)duration)})";
            }
        }

        public virtual List<ITechOperation> CreateTechOperations() => new List<ITechOperation>();

        public virtual bool Validate() => true;

        public virtual void Teardown()
        {
            Acad.DeleteObjects(OriginObject);
            TechOperations.ForEach(to => to.Teardown());
        }
    }
}