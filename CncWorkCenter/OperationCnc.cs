using Autodesk.AutoCAD.DatabaseServices;
using System;
using Autodesk.AutoCAD.Geometry;
using CAM.CncWorkCenter;
using CAM.Core;

namespace CAM
{
    [Serializable]
    public abstract class OperationCnc: ProcessItem
    {
        public override MachineType MachineType => MachineType.CncWorkCenter;
        public double Duration { get; set; }
        public AcadObject ProcessingArea { get; set; }

        [NonSerialized] public ObjectId? ToolpathGroup;
        [NonSerialized] public ObjectId? SupportGroup;
        [NonSerialized] public int FirstCommandIndex;
        [NonSerialized] public ProcessingCnc Processing;

        public Machine Machine => Processing.Machine.Value;
        public Tool Tool => Processing.Tool;
        public double ToolDiameter => Processing.Tool.Diameter;
        public double ToolThickness => Processing.Tool.Thickness.Value;
        public int CuttingFeed => Processing.CuttingFeed;
        public int PenetrationFeed => Processing.PenetrationFeed;
        public double ZSafety => Processing.ZSafety;
        public Point2d Origin => Processing.Origin;

        public virtual void Init() { }

        public virtual void Teardown() { }

        public abstract void Execute(ProcessorCnc processor);

        public void RemoveAcadObjects()
        {
            ToolpathGroup?.DeleteGroup();
            ToolpathGroup = null;
            SupportGroup?.DeleteGroup();
            SupportGroup = null;
        }
    }
}
