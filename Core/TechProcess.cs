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
    public abstract class TechProcess
    {
        public string Caption { get; set; }

        public MachineType? MachineType { get; set; }

        public Material? Material { get; set; }

        public double? Thickness { get; set; }

        public Tool Tool { get; set; }

        public int Frequency { get; set; }

        public int PenetrationFeed { get; set; }

        public double ZSafety { get; set; } = 20;

        public List<AcadObject> ProcessingArea { get; set; }

        public List<TechOperation> TechOperations { get; } = new List<TechOperation>();

        public List<ProcessCommand> ProcessCommands { get; set; }

        public double OriginX { get; set; }

        public double OriginY { get; set; }

        [NonSerialized]
        public ObjectId[] OriginObject;
        [NonSerialized]
        public Dictionary<ObjectId, int> ToolpathObjectIds;
        [NonSerialized]
        public ObjectId? ToolpathObjectsGroup;
        [NonSerialized]
        public ObjectId? ExtraObjectsGroup;
       
        public virtual void SerializeInit()
        {
            if (OriginX != 0 || OriginY != 0)
                OriginObject = Acad.CreateOriginObject(new Point3d(OriginX, OriginY, 0));

            AcadObject.LoadAcadProps(this);

            TechOperations.ForEach(p =>
            {
                AcadObject.LoadAcadProps(p);
                p.TechProcessBase = this;
                p.SerializeInit();
            });
        }

        protected virtual void SetTool(CommandGeneratorBase generator) => generator.SetTool(MachineType.Value != CAM.MachineType.Donatoni ? Tool.Number : 1, Frequency);

        public virtual void BuildProcessing()
        {
            using (var generator = CommandGeneratorFactory.Create(MachineType.Value))
            {
                generator.StartTechProcess(this);

                if (Tool != null)
                    SetTool(generator);

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
                ProcessCommands = generator.ProcessCommands;
            }
            UpdateFromCommands();
        }

        protected virtual void BuildProcessing(CommandGeneratorBase generator) { }

        public virtual void SkipProcessing(ProcessCommand processCommand)
        {
            if (!(processCommand.Owner is TechOperation techOperation))
                return;

            var objIds = ProcessCommands.SkipWhile(p => p != processCommand).Select(p => p.ToolpathObjectId).Distinct();
            ProcessCommands.Select(p => p.ToolpathObjectId).Except(objIds).Delete();

            ToolpathObjectsGroup?.Delete();
            ToolpathObjectsGroup = null;

            TechOperations.Select(p => p.ToolpathObjectsGroup).Delete();
            TechOperations.ForEach(p =>
            {
                p.ToolpathObjectsGroup = null;
                p.ProcessCommandIndex = null;
            });
            ToolpathObjectIds = null;

            using (var generator = CommandGeneratorFactory.Create(MachineType.Value))
            {
                generator.StartTechProcess(this);
                generator.SetTool(
                    MachineType.Value != CAM.MachineType.Donatoni ? Tool.Number : 1,
                    Frequency);

                generator.SetTechOperation(techOperation);
                techOperation.PrepareBuild(generator);

                var loc = ProcessCommands[ProcessCommands.IndexOf(processCommand) - 1].ToolLocation;
                generator.Move(loc.Point.X, loc.Point.Y, angleC: loc.AngleC, angleA: loc.AngleA);
                generator.GCommand(CommandNames.Penetration, 1, point: loc.Point, feed: PenetrationFeed);

                ProcessCommands = generator.ProcessCommands.Concat(ProcessCommands.SkipWhile(p => p != processCommand)).ToList();
            }
            UpdateFromCommands();
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
                if (group.Key is TechOperation techOperation)
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

        public void DeleteProcessing()
        {
            ToolpathObjectsGroup?.DeleteGroup();
            ToolpathObjectsGroup = null;

            TechOperations.Select(p => p.ToolpathObjectsGroup).Delete();
            TechOperations.ForEach(p =>
            {
                p.ToolpathObjectsGroup = null;
                p.ProcessCommandIndex = null;
            });
            ExtraObjectsGroup?.DeleteGroup();
            ExtraObjectsGroup = null;

            ToolpathObjectIds = null;
            ProcessCommands = null;
        }

        public virtual List<TechOperation> CreateTechOperations() => new List<TechOperation>();

        public virtual bool Validate() => true;

        public virtual void Teardown()
        {
            Acad.DeleteObjects(OriginObject);
            TechOperations.ForEach(to => to.Teardown());
        }

    }
}