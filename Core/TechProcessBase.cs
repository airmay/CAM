using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    /// <summary>
    /// Технологический процесс обработки
    /// </summary>
    [Serializable]
    public abstract class TechProcess<T> : ITechProcess where T : CommandGeneratorBase
    {
        public string Caption { get; set; }

        public Machine? MachineType { get; set; }

        public CAM.Tool Tool { get; set; }

        #region TechOperations

        private List<TechOperation<T>> _techOperations = new List<TechOperation<T>>();

        public IEnumerable<TechOperation> TechOperations => _techOperations;

        public void AddTechOperation(TechOperation<T> techOperation) => _techOperations.Add(techOperation);

        public void RemoveTechOperation(TechOperation<T> techOperation) => _techOperations.Remove(techOperation);

        public bool MoveForwardTechOperation(TechOperation<T> techOperation) => _techOperations.SwapNext(techOperation);

        public bool MoveBackwardTechOperation(TechOperation<T> techOperation) => _techOperations.SwapPrev(techOperation);

        #endregion

        public double ZSafety { get; set; } = 20;

        public AcadObject ProcessingArea { get; set; }

        [NonSerialized]
        private List<ProcessCommand> _processCommands = new List<ProcessCommand>();

        public List<ProcessCommand> ProcessCommands => _processCommands;

        public double OriginX { get; set; }

        public double OriginY { get; set; }

        public int GetFirstCommandIndex() => 0;

        [NonSerialized]
        public ObjectId[] OriginObject;
        [NonSerialized]
        public Dictionary<ObjectId, int> ToolpathObjectIds;
        [NonSerialized]
        public ObjectId? ToolpathObjectsGroup;
        [NonSerialized]
        public ObjectId? ExtraObjectsGroup;

        public Dictionary<ObjectId, int> GetToolpathObjectIds() => ToolpathObjectIds;

        public ObjectId? GetToolpathObjectsGroup() => ToolpathObjectsGroup;

        public ObjectId? GetExtraObjectsGroup() => ExtraObjectsGroup;

        public void Select() => ToolpathObjectsGroup?.SetGroupVisibility(true);

        public virtual void SerializeInit()
        {
            //if (OriginX != 0 || OriginY != 0)
            //    OriginObject = Acad.CreateOriginObject(new Point3d(OriginX, OriginY, 0));

            //AcadObject.LoadAcadProps(this);

            //_techOperations.ForEach(p =>
            //{
            //    AcadObject.LoadAcadProps(p);
            //    //p.TechProcessBase = this;
            //    p.SerializeInit();
            //});
        }

        public virtual int GetFrequency() => 0;

        public virtual void BuildProcessing()
        {
            using (var generator = CommandGeneratorFactory.Create<T>(MachineType.Value))
            {
                generator.StartTechProcess(this);

                //if (generator is MillingCommandGenerator millingCommandGenerator && Tool != null)
                //    millingCommandGenerator.SetTool(
                //        Machine.Value != CAM.Machine.Donatoni ? Tool.Number : 1,
                //        GetFrequency());

                BuildProcessing(generator);

                _techOperations.FindAll(p => p.Enabled && p.CanProcess).ForEach(p =>
                {
                    generator.SetTechOperation(p);

                    //    p.PrepareBuild(generator);
                    p.BuildProcessing(generator);

                    p.PostBuild(generator);
                    //    if (!generator.IsUpperTool)
                    //        generator.Uplifting();
                });
                generator.FinishTechProcess();
                _processCommands = generator.ProcessCommands;
            }
            UpdateFromCommands();
        }

        protected virtual void BuildProcessing(T generator) { }

        public virtual void SkipProcessing(ProcessCommand processCommand)
        {
            //if (!(processCommand.Owner is TechOperation techOperation))
            //    return;

            //var objIds = ProcessCommands.SkipWhile(p => p != processCommand).Select(p => p.ToolpathObjectId).Distinct();
            //ProcessCommands.Select(p => p.ToolpathObjectId).Except(objIds).Delete();

            //ToolpathObjectsGroup?.Delete();
            //ToolpathObjectsGroup = null;

            //TechOperations.Select(p => p.ToolpathObjectsGroup).Delete();
            //TechOperations.ForEach(p =>
            //{
            //    p.ToolpathObjectsGroup = null;
            //    p.ProcessCommandIndex = null;
            //});
            //ToolpathObjectIds = null;

            //using (var generator = CommandGeneratorFactory.Create(Machine.Value))
            //{
            //    generator.StartTechProcess(this);
            //    generator.SetTool(
            //        Machine.Value != CAM.Machine.Donatoni ? Tool.Number : 1,
            //        Frequency);

            //    generator.SetTechOperation(techOperation);
            //    techOperation.PrepareBuild(generator);

            //    var loc = ProcessCommands[ProcessCommands.IndexOf(processCommand) - 1].ToolLocation;
            //    //generator.Move(loc.Point.X, loc.Point.Y, angleC: loc.AngleC, angleA: loc.AngleA);
            //    generator.GCommand(CommandNames.Penetration, 1, point: loc.Point, feed: PenetrationFeed);

            //    ProcessCommands = generator.ProcessCommands.Concat(ProcessCommands.SkipWhile(p => p != processCommand)).ToList();
            //}
            //UpdateFromCommands();
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

            _techOperations.Select(p => p.ToolpathObjectsGroup).Delete();
            _techOperations.ForEach(p =>
            {
                p.ToolpathObjectsGroup = null;
                p.FirstCommandIndex = null;
            });
            ExtraObjectsGroup?.DeleteGroup();
            ExtraObjectsGroup = null;

            ToolpathObjectIds = null;
            _processCommands = null;
        }

        public virtual List<TechOperation> CreateTechOperations() => new List<TechOperation>();

        public virtual bool Validate() => true;

        public virtual void Teardown()
        {
            Acad.DeleteObjects(OriginObject);
            _techOperations.ForEach(to => to.Teardown());
        }
    }
}
