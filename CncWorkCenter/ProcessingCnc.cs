using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Core;

namespace CAM.CncWorkCenter
{
    [Serializable]
    public class ProcessingCnc : ProcessingBase
    {
        private static Program<CommandCnc> _program = new Program<CommandCnc>();
        public override IProgram Program => _program;
        
        private ProcessorCnc _processor;
        public IEnumerable<IOperation> Operations => OperationsArray;
        public OperationCnc[] OperationsArray { get; set; }

        public virtual MachineType MachineType { get; set; }
        public Machine? Machine { get; set; }
        public Material? Material { get; set; }
        
        public Tool Tool { get; set; }
        public int Frequency { get; set; }
        
        public int CuttingFeed { get; set; }
        public int PenetrationFeed { get; set; }
        public double ZSafety { get; set; } = 20;

        public double OriginX { get; set; }
        public double OriginY { get; set; }
        public Point2d Origin => new Point2d(OriginX, OriginX);
        [NonSerialized] public ObjectId? OriginGroup;

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddMachine(CAM.Machine.Donatoni, CAM.Machine.ScemaLogic, CAM.Machine.Forma);
            view.AddMaterial();
            view.AddIndent();
            view.AddTool();
            view.AddTextBox(nameof(Frequency));
            view.AddIndent();
            view.AddTextBox(nameof(CuttingFeed));
            view.AddTextBox(nameof(PenetrationFeed));
            view.AddIndent();
            view.AddOrigin();
            view.AddTextBox(nameof(ZSafety));
        }

        protected override void ProcessOperations()
        {
            if (!Machine.CheckNotNull("Станок") || !Tool.CheckNotNull("Инструмент"))
                return;

            using (var processor = ProcessorFactory.Create(this))
            {
                processor.Start();
                foreach (OperationCnc operation in Children.Where(p => p.Enabled))
                {
                    Acad.Write($"расчет операции {operation.Caption}");

                    processor.SetOperation(operation);
                    operation.Processing = this;
                    operation.Execute(processor);
                }
            }
        }

        //public void RemoveAcadObjects()
        //{
        //    foreach (var operation in Operations)
        //        operation.RemoveAcadObjects();
        //}
    }
}