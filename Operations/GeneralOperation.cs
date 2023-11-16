using Autodesk.AutoCAD.DatabaseServices;
using System;
using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    [Serializable]
    public class GeneralOperation : OperationBase
    {
        public Operation[] Operations { get; set; }

        public MachineType? MachineType { get; set; }
        public Material? Material { get; set; }
        
        public Tool Tool { get; set; }
        public int Frequency { get; set; }
        
        public int CuttingFeed { get; set; }
        public int PenetrationFeed { get; set; }
        public double ZSafety { get; set; } = 20;

        public double OriginX { get; set; }
        public double OriginY { get; set; }
        [NonSerialized] public ObjectId[] Origin;

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddMachine(CAM.MachineType.Donatoni, CAM.MachineType.ScemaLogic, CAM.MachineType.Forma);
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

        public virtual void Init()
        {
            if (OriginX != 0 || OriginY != 0)
                Origin = Acad.CreateOriginObject(new Point3d(OriginX, OriginY, 0));

            foreach (var operation in Operations)
            {
                AcadObject.LoadAcadProps(operation);
                operation.Init();
            }
        }
    }
}