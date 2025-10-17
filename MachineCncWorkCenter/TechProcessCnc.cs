using System;
using CAM.Core;
using CAM.Core.Enums;
using CAM.UI;

namespace CAM.MachineCncWorkCenter;

[Serializable]
public class TechProcessCnc : TechProcessBase<TechProcessCnc, ProcessorCnc>
{
    public Material? Material { get; set; }
    public int Frequency { get; set; }
    public int CuttingFeed { get; set; }
    public int PenetrationFeed { get; set; }

    public static void ConfigureParamsView(ParamsControl view)
    {
        view.AddMachine();
        view.AddMaterial();

        view.AddIndent();
        view.AddTool();
        view.AddTextBox(nameof(Frequency));
        view.AddTextBox(nameof(CuttingFeed), required: true);
        view.AddTextBox(nameof(PenetrationFeed), required: true);

        view.AddIndent();
        view.AddOrigin();
        view.AddTextBox(nameof(Delta));
        view.AddTextBox(nameof(ZSafety));
    }

    public TechProcessCnc()
    {
        Caption = "Обработка ЧПУ";
    }
}