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

            view.AddMachine(CAM.MachineType.Donatoni, CAM.MachineType.ScemaLogic, CAM.MachineType.Forma, CAM.MachineType.Champion)
                .AddMaterial()
                .AddParam(nameof(Thickness))
                .AddIndent()
                .AddTool()
                .AddParam(nameof(Frequency))
                .AddParam(nameof(PenetrationFeed))
                .AddIndent()
                .AddAcadObject(allowedTypes: $"{AcadObjectNames.Surface},{AcadObjectNames.Region}", afterSelect: (ids) => refreshSize())
                .AddText("Размеры", p => sizeLabel = p)
                .AddParam(nameof(Angle), "Угол")
                .AddParam(nameof(IsExactlyBegin), "Начало точно")
                .AddParam(nameof(IsExactlyEnd), "Конец точно")
                .AddParam(nameof(ZSafety))
                .AddParam(nameof(IsA90), "Угол A = 90")
                .AddParam(nameof(AngleA), "Угол вертикальный")
                //.AddParam(nameof(DzBillet), "dZ заготовки")
                .AddParam(nameof(OriginCellNumber), "Ячейка начала координат");
        }

        protected override void SetTool(MillingCommandGenerator generator) =>
            generator.SetTool(
                MachineType.Value != CAM.MachineType.Donatoni ? Tool.Number : 1,
                Frequency,
                angleA: IsA90 ? 90 : AngleA,
                angleC: Angle,
                originCellNumber: OriginCellNumber);
    }
}
