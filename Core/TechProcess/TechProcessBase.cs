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
    public abstract class TechProcessBase : ITechProcess
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
        [NonSerialized]
        private Dictionary<ObjectId, int> _toolpathObjectIds;
        [NonSerialized]
        private ObjectId? _toolpathObjectsGroup;
        [NonSerialized]
        private ObjectId? _extraObjectsGroup;

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

        public Dictionary<ObjectId, int> ToolpathObjectIds { get => _toolpathObjectIds; set => _toolpathObjectIds = value; }

        public ObjectId? ToolpathObjectsGroup { get => _toolpathObjectsGroup; set => _toolpathObjectsGroup = value; }

        public ObjectId? ExtraObjectsGroup { get => _extraObjectsGroup; set => _extraObjectsGroup = value; }

        public bool TechOperationMoveDown(ITechOperation techOperation) => TechOperations.SwapNext(techOperation);

        public bool TechOperationMoveUp(ITechOperation techOperation) => TechOperations.SwapPrev(techOperation);

        public virtual void BuildProcessing()
        {
            using (var generator = CommandGeneratorFactory.Create(MachineType.Value))
            {
                generator.StartTechProcess(this);

                if (Tool != null)
                    generator.SetTool(
                        MachineType.Value != CAM.MachineType.Donatoni ? Tool.Number : 1,
                        Frequency);

                BuildProcessing(generator);

                TechOperations.FindAll(p => p.Enabled && p.CanProcess).ForEach(p =>
                {
                    generator.SetTechOperation(p);

                    p.PrepareBuild(generator);
                    p.BuildProcessing(generator);

                    if (!generator.IsUpperTool)
                        generator.Uplifting();
                });
                generator.FinishTechProcess();
            }
            UpdateFromCommands();
        }

        protected virtual void BuildProcessing(CommandGeneratorBase generator) { }

        public virtual void SkipProcessing(ProcessCommand processCommand)
        {
            //var techOperation = TechOperations.FirstOrDefault(p => p.ProcessCommands?.Contains(processCommand) == true);
            //if (techOperation == null)
            //    return;

            //List<ProcessCommand> techOperationCommands;
            //List<ProcessCommand> techProcessCommands;
            //using (var generator = CommandGeneratorFactory.Create(MachineType.Value))
            //{
            //    generator.StartTechProcess(Caption, OriginX, OriginY, ZSafety);
            //    generator.StartTechOperation();
            //    generator.SetTool(
            //        MachineType.Value != CAM.MachineType.Donatoni ? Tool.Number : processCommand.HasTool ? 1 : 2, 
            //        Frequency);
            //    techOperation.PrepareBuild(generator);
            //    var loc = ProcessCommands[ProcessCommands.IndexOf(processCommand) - 1].ToolLocation;
            //    generator.Move(loc.Point.X, loc.Point.Y, angleC: loc.AngleC, angleA: loc.AngleA);
            //    generator.GCommand(CommandNames.Penetration, 1, point: loc.Point, feed: PenetrationFeed);
            //    techOperationCommands = generator.FinishTechOperation();
            //    techProcessCommands = generator.FinishTechProcess();
            //}
            //TechOperations.TakeWhile(p => p != techOperation).ToList().ForEach(p => p.ProcessCommands = null);
            //techOperation.ProcessCommands = techOperationCommands.Concat(techOperation.ProcessCommands.SkipWhile(p => p.Number < processCommand.Number)).ToList();
            //ProcessCommands = techProcessCommands.TakeWhile(p => !techOperationCommands.Contains(p)).Concat(techOperationCommands)
            //    .Concat(ProcessCommands.SkipWhile(p => p.Number < processCommand.Number)).ToList();
            //UpdateCaptions();
        }

        private void UpdateFromCommands()
        {
            ToolpathObjectIds = ProcessCommands.Select((command, index) => new { command, index })
                .Where(p => p.command.ToolpathObjectId.HasValue)
                .GroupBy(p => p.command.ToolpathObjectId.Value)
                .ToDictionary(p => p.Key, p => p.Min(k => k.index));
            ToolpathObjectsGroup = ProcessCommands.Select(p => p.ToolpathObjectId).CreateGroup();
            Caption = GetCaption(Caption, ProcessCommands.Sum(p => p.Duration));
            foreach (var group in ProcessCommands.GroupBy(p => p.Owner))
                if (group.Key is ITechOperation techOperation)
                {
                    techOperation.ToolpathObjectsGroup = group.Select(p => p.ToolpathObjectId).CreateGroup();
                    techOperation.Caption = GetCaption(techOperation.Caption, group.Sum(p => p.Duration));
                }

            string GetCaption(string caption, double duration)
            {
                var ind = caption.IndexOf('(');
                return $"{(ind > 0 ? caption.Substring(0, ind).Trim() : caption)} ({new TimeSpan(0, 0, 0, (int)duration)})";
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