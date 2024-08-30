using Autodesk.AutoCAD.DatabaseServices;
using System;
using Autodesk.AutoCAD.Geometry;
using CAM.CncWorkCenter;
using CAM.Core;

namespace CAM
{
    [Serializable]
    [MachineTypeNew(MachineType.CncWorkCenter)]
    public abstract class OperationCnc : OperationBase, IOperation
    {
        public double Duration { get; set; }
        public AcadObject ProcessingArea { get; set; }

        [NonSerialized] public ObjectId? ToolpathGroup;
        [NonSerialized] public ObjectId? SupportGroup;
        [NonSerialized] public int FirstCommandIndex;
        [NonSerialized] public ProcessingCnc Processing;

        public Machine Machine => Processing.Machine.Value;
        public Tool Tool => Processing.Tool;
        public int CuttingFeed => Processing.CuttingFeed;
        public int PenetrationFeed => Processing.PenetrationFeed;
        public double ZSafety => Processing.ZSafety;
        public Point2d Origin => Processing.Origin;

        public virtual void Init() { }
        public OperationCnc CreateOperation(Type type, OperationCnc prototype)
        {
            throw new NotImplementedException();
        }

        public virtual void Teardown() { }

        public abstract void Execute(ProcessorCnc processor);

        public OperationCnc[] Operations { get; set; }

        public void Execute()
        {
            throw new NotImplementedException();
        }

        public void RemoveAcadObjects()
        {
            ToolpathGroup?.DeleteGroup();
            ToolpathGroup = null;
            SupportGroup?.DeleteGroup();
            SupportGroup = null;
        }
    }
}
