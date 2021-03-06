﻿using System;
using System.Windows.Forms;

namespace CAM.TechProcesses.Disk3D
{
    [Serializable]
    [MenuItem("Диск 3D", 3)]
    public class Disk3DTechProcess : TechProcess
    {
        public double Angle { get; set; }

        public bool IsExactlyBegin { get; set; }

        public bool IsExactlyEnd { get; set; }

        public static void ConfigureParamsView(ParamsView view)
        {
            Label sizeLabel = null;
            void refreshSize() => sizeLabel.Text = Acad.GetSize(view.GetParams<Disk3DTechProcess>().ProcessingArea);
            view.BindingSource.DataSourceChanged += (s, e) => refreshSize();

            view.AddMachine(CAM.MachineType.Donatoni, CAM.MachineType.ScemaLogic)
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
                .AddParam(nameof(ZSafety));
        }
    }
}
