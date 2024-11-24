using System;

namespace CAM.CncWorkCenter
{
    [Serializable]
    public class ProcessingWireSaw : ProcessingBase
    {
        public override MachineType MachineType => MachineType.WireSawMachine;
        [NonSerialized] public ProcessorWireSaw Processor;

        public Tool Tool { get; set; }
        public double ToolThickness { get; set; } = 10;
        public int CuttingFeed { get; set; } = 10;
        public int S { get; set; } = 100;
        public double Approach { get; set; }
        public double Departure { get; set; } = 50;
        public double Delta { get; set; } = 0;
        public double Delay { get; set; } = 60;
        public bool IsExtraMove { get; set; }
        public bool IsExtraRotate { get; set; }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddOrigin();
            view.AddIndent();
            view.AddTextBox(nameof(CuttingFeed));
            view.AddTextBox(nameof(S), "Угловая скорость", toolTipText: "Z безопасности отсчитывается от верха выбранных объектов техпроцесса");;
            view.AddIndent();
            view.AddTextBox(nameof(Approach), "Заезд");
            view.AddTextBox(nameof(Departure), "Выезд");
            view.AddIndent();
            view.AddTextBox(nameof(ToolThickness), "Толщина троса");
            view.AddTextBox(nameof(Delta));
            view.AddTextBox(nameof(Delay), "Задержка");
            view.AddTextBox(nameof(IsExtraMove), "Возврат");
            view.AddTextBox(nameof(IsExtraRotate), "Поворот");
        }

        public ProcessingWireSaw()
        {
            Caption = "Обработка Тросовый станок";
            Machine = CAM.Machine.WireSawMachine;
        }

        protected override IProcessor CreateProcessor()
        {
            return new ProcessorWireSaw(this, new PostProcessorWireSaw());
        }

        protected override bool Validate()
        {
            return Tool.CheckNotNull("Инструмент");
        }
    }
}