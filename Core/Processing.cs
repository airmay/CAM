using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;

namespace CAM.Core
{
    public class Processing
    {
        public string Caption { get; set; }
        public List<Operation> Operations { get; set; }
        public MachineType? MachineType { get; set; }
        public CAM.Tool Tool { get; set; }
        public double ZSafety { get; set; } = 20;

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
            if (OriginX != 0 || OriginY != 0)
                OriginObject = Acad.CreateOriginObject(new Point3d(OriginX, OriginY, 0));

            Operations.ForEach(p =>
            {
                AcadObject.LoadAcadProps(p);
                //p.TechProcessBase = this;
                p.SerializeInit();
            });
        }

        public virtual void BuildProcessing()
        {
            var processor = new Processor(this);
            processor.Start();
            Operations.FindAll(p => p.Enabled)
                .ForEach(p =>
                {
                    processor.SetOperarion(p);
                    p.Execute(processor);
                });
            processor.Finish();
            _processCommands = processor.ProcessCommands;

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

            Operations.Select(p => p.ToolpathObjectsGroup).Delete();
            Operations.ForEach(p =>
            {
                p.ToolpathObjectsGroup = null;
                p.FirstCommandIndex = null;
            });
            ExtraObjectsGroup?.DeleteGroup();
            ExtraObjectsGroup = null;

            ToolpathObjectIds = null;
            _processCommands = null;
        }

        public virtual bool Validate() => true;

        public virtual void Teardown()
        {
            Acad.DeleteObjects(OriginObject);
            Operations.ForEach(to => to.Teardown());
        }
    }
}
