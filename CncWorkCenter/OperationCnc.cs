using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.CncWorkCenter;

namespace CAM
{
    [Serializable]
    public abstract class OperationCnc : OperationBase
    {
        public ProcessingCnc Processing => (ProcessingCnc)ProcessingBase;
        protected ProcessorCnc Processor => Processing.Processor;

        public override MachineType MachineType => MachineType.CncWorkCenter;
        public override Machine Machine => Processing.Machine.Value;
        public override Tool Tool => Processing.Tool;
        public double ToolDiameter => Processing.Tool.Diameter;
        public double ToolThickness => Processing.Tool.Thickness.Value;
        public int CuttingFeed => Processing.CuttingFeed;
        public int PenetrationFeed => Processing.PenetrationFeed;
        public double ZSafety => Processing.ZSafety;
        public Point2d Origin => Processing.Origin;

        public void Update(List<ObjectId?> objectIds)
        {
            ToolpathGroup = objectIds.CreateGroup();
        }
    }
}
