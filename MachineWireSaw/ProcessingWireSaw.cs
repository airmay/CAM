using System;
using CAM.MachineWireSaw;

namespace CAM.CncWorkCenter
{
    [Serializable]
    public class ProcessingWireSaw : ProcessingBase<ProcessingWireSaw, ProcessorWireSaw>
    {
        public double ToolThickness
        {
            get => Tool.Thickness.GetValueOrDefault();
            set => Tool.Thickness = value;
        }
        public int CuttingFeed { get; set; } = 10;
        public int S { get; set; } = 100;
        public double Approach { get; set; } = 50;
        public double Departure { get; set; } = 50;
        public double Delta { get; set; } = 5;

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddOrigin();
            view.AddTextBox(nameof(CuttingFeed));
            view.AddTextBox(nameof(S), "Угловая скорость",
                toolTipText: "Z безопасности отсчитывается от верха выбранных объектов техпроцесса");
            
            view.AddIndent();
            view.AddTextBox(nameof(ToolThickness), "Толщина троса");
            view.AddTextBox(nameof(Approach), "Заезд");
            view.AddTextBox(nameof(Departure), "Выезд");

            view.AddIndent();
            view.AddTextBox(nameof(Delta));
            view.AddTextBox(nameof(ZSafety));
        }

        public double Offset => ToolThickness / 2 + Delta;

        public ProcessingWireSaw()
        {
            Caption = "Обработка Тросовый станок";
            Machine = CAM.Machine.WireSawMachine;
            Tool = new Tool { Type = ToolType.WireSaw };
        }

        protected override bool Validate()
        {
            return Tool.Thickness.CheckNotNull("Толщина троса");
        }
    }
}