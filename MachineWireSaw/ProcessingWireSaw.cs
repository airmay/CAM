using System;
using CAM.MachineWireSaw;

namespace CAM.CncWorkCenter;

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

    public static void ConfigureParamsView(ParamsControl view)
    {
        view.AddTextBox(nameof(ToolThickness), "Толщина троса", required: true);
        view.AddTextBox(nameof(CuttingFeed), required: true);
        view.AddTextBox(nameof(S), "Угловая скорость", required: true);

        view.AddIndent();
        view.AddTextBox(nameof(Approach), "Заезд");
        view.AddTextBox(nameof(Departure), "Выезд");

        view.AddIndent();
        view.AddOrigin();
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
}