using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Core;

namespace CAM.CncWorkCenter
{
    [Serializable]
    public class ProcessingCnc : OperationBase, IProcessing
    {
        public List<OperationCnc> Operations { get; set; } = new List<OperationCnc>();

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

        public void Init()
        {
            if (Origin != Point2d.Origin)
                OriginGroup = Acad.CreateOriginObject(Origin);

            foreach (var operation in Operations)
            {
                AcadObject.LoadAcadProps(operation);
                operation.Init();
            }
        }

        public void Teardown()
        {
            foreach (var operation in Operations)
                operation.Teardown();
        }

        public void Execute()
        {
            if (!Machine.CheckNotNull("Станок") || !Tool.CheckNotNull("Инструмент"))
                return;

            using (var processor = ProcessorFactory.Create(Machine.Value))
            {
                processor.Start(Tool);
                processor.SetGeneralOperarion(this);
                foreach (var operation in Operations.Where(p => p.Enabled))
                {
                    Acad.Write($"расчет операции {operation.Caption}");

                    processor.SetOperation(operation);
                    operation.Processing = this;
                    operation.Execute(processor);
                }

                processor.Finish();
            }
        }

        public void RemoveAcadObjects()
        {
            foreach (var operation in Operations)
                operation.RemoveAcadObjects();
        }
    }
}