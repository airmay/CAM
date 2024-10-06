using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.CncWorkCenter;
using CAM.Core;

namespace CAM
{
    [Serializable]
    public abstract class OperationCnc : Operation
    {
        public override MachineType MachineType => MachineType.CncWorkCenter;
        [NonSerialized] public ProcessingCnc Processing;

        public override Machine Machine => Processing.Machine.Value;
        public override Tool Tool => Processing.Tool;
        public double ToolDiameter => Processing.Tool.Diameter;
        public double ToolThickness => Processing.Tool.Thickness.Value;
        public int CuttingFeed => Processing.CuttingFeed;
        public int PenetrationFeed => Processing.PenetrationFeed;
        public double ZSafety => Processing.ZSafety;
        public Point2d Origin => Processing.Origin;


        public virtual void Init()
        {
        }

        public virtual void Teardown()
        {
        }

        public abstract void Execute(ProcessorCnc processor);

        public void RemoveAcadObjects()
        {
            ToolpathGroup?.DeleteGroup();
            ToolpathGroup = null;
            SupportGroup?.DeleteGroup();
            SupportGroup = null;
        }

        public void Update(List<ObjectId?> objectIds)
        {
            ToolpathGroup = objectIds.CreateGroup();
        }
    }
}
