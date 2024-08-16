using System;
using System.Windows.Forms;

namespace CAM.TechProcesses.Disk3D
{
    [Serializable]
    [MenuItem("Диск 3D", 3)]
    public class Disk3DTechProcess : MillingTechProcess
    {
        public double Angle { get; set; }

        public bool IsExactlyBegin { get; set; }

        public bool IsExactlyEnd { get; set; }

        public bool IsA90 { get; set; }

        public int OriginCellNumber { get; set; }

        public double AngleA { get; set; }

        //public double DzBillet { get; set; }

        public static void ConfigureParamsView(ParamsView view)
        {
            Label sizeLabel = null;
            void refreshSize() => sizeLabel.Text = Acad.GetSize(view.GetParams<Disk3DTechProcess>().ProcessingArea);
            view.BindingSource.DataSourceChanged += (s, e) => refreshSize();

            view.AddMachine(CAM.Machine.Donatoni, CAM.Machine.ScemaLogic, CAM.Machine.Forma, CAM.Machine.Champion);
            view.AddMaterial();
            view.AddTextBox(nameof(Thickness));
            view.AddIndent();
            view.AddTool();
            view.AddTextBox(nameof(Frequency));
            view.AddTextBox(nameof(PenetrationFeed));
            view.AddIndent();
            view.AddAcadObject(allowedTypes: $"{AcadObjectNames.Surface},{AcadObjectNames.Region}", afterSelect: (ids) => refreshSize());
            sizeLabel = view.AddLabelText("Размеры");
            view.AddTextBox(nameof(Angle), "Угол");
            view.AddCheckBox(nameof(IsExactlyBegin), "Начало точно");
            view.AddCheckBox(nameof(IsExactlyEnd), "Конец точно");
            view.AddTextBox(nameof(ZSafety));
            view.AddCheckBox(nameof(IsA90), "Угол A = 90");
            view.AddTextBox(nameof(AngleA), "Угол вертикальный");
            //.AddTextBox(nameof(DzBillet), "dZ заготовки")
            view.AddTextBox(nameof(OriginCellNumber), "Ячейка начала координат");
        }

        protected override void SetTool(MillingCommandGenerator generator) =>
            generator.SetTool(
                MachineType.Value != CAM.Machine.Donatoni ? Tool.Number : 1,
                Frequency,
                angleA: IsA90 ? 90 : AngleA,
                angleC: Angle,
                originCellNumber: OriginCellNumber);
    }
}
