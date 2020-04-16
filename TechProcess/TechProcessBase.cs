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

        public int PenetrationFeed { get; set; }

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
            if (!Validate() || TechOperations.Any(p => p.Enabled && p.CanProcess && !p.Validate()))
                return;
            DeleteProcessCommands();

            using (var generator = CommandGeneratorFactory.Create(MachineType.Value))
            {
                generator.StartTechProcess(Caption, OriginX, OriginY, zSafety);
                TechOperations.FindAll(p => p.Enabled && p.CanProcess).ForEach(p =>
                {
                    generator.StartTechOperation();
                    if (Tool != null)
                        generator.SetTool(MachineType.Value == CAM.MachineType.ScemaLogic ? Tool.Number : 1, Frequency);
                    p.BuildProcessing(generator);
                    if (!generator.IsUpperTool)
                        generator.Uplifting();
                    p.ProcessCommands = generator.FinishTechOperation();                    
                });
                ProcessCommands = generator.FinishTechProcess();
            }
            UpdateCaptions();
        }

        public virtual void SkipProcessing(ProcessCommand processCommand, int zSafety)
        {
            List<ProcessCommand> techOperationCommands;
            List<ProcessCommand> techProcessCommands;
            using (var generator = CommandGeneratorFactory.Create(MachineType.Value))
            {
                generator.StartTechProcess(Caption, OriginX, OriginY, zSafety);
                generator.StartTechOperation();
                generator.SetTool(processCommand.ToolNumber, Frequency);
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
            var duration = 0D;
            TechOperations.ForEach(op =>
            {
                var d = op.ProcessCommands?.Sum(p => p.Duration) ?? 0;
                op.Caption = GetCaption(op.Caption, d);
                duration += d;
            });
            Caption = GetCaption(Caption, duration);

            string GetCaption(string text, double value)
            {
                var ind = text.IndexOf('(');
                return $"{(ind > 0 ? text.Substring(0, ind).Trim() : text)} ({new TimeSpan(0, 0, 0, (int)value)})";
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