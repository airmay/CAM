using System;
using CAM.MachineWireSaw;

namespace CAM.CncWorkCenter
{
    [Serializable]
    public class ProcessingWireSaw : ProcessingBase
    {
        public override MachineType MachineType => MachineType.WireSawMachine;
        [NonSerialized] public ProcessorWireSaw Processor;

        public ToolWireSaw Tool { get; } = new ToolWireSaw();

        public double ToolThickness
        {
            get => Tool.Thickness;
            set => Tool.Thickness = value;
        }

        public int CuttingFeed { get; set; } = 10;
        public int S { get; set; } = 100;
        public double Approach { get; set; }
        public double Departure { get; set; } = 50;
        public double Delta { get; set; } = 0;


        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddOrigin();
            view.AddTextBox(nameof(CuttingFeed));
            view.AddTextBox(nameof(S), "Угловая скорость", toolTipText: "Z безопасности отсчитывается от верха выбранных объектов техпроцесса");;
            view.AddIndent();
            view.AddTextBox(nameof(ToolThickness), "Толщина троса");
            view.AddTextBox(nameof(Delta));
            view.AddIndent();
            view.AddTextBox(nameof(Approach), "Заезд");
            view.AddTextBox(nameof(Departure), "Выезд");
        }

        public ProcessingWireSaw()
        {
            Caption = "Обработка Тросовый станок";
            Machine = CAM.Machine.WireSawMachine;
        }

        protected override IProcessor CreateProcessor()
        {
            Processor = new ProcessorWireSaw(this, new PostProcessorWireSaw());
            return Processor;
        }

        protected override bool Validate()
        {
            return ToolThickness.CheckNotNull("Толщина троса");
        }
    }
}