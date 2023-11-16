﻿using CAM.TechProcesses.Sawing;
using System;
using System.Collections.Generic;

namespace CAM.Operations.Sawing
{
    [Serializable]
    public class SawingOperation : Operation
    {
        public Side OuterSide { get; set; }

        //public double? Thickness { get; set; }
        public bool IsExactlyBegin { get; set; }
        public bool IsExactlyEnd { get; set; }
        public double AngleA { get; set; }
        public double Departure { get; set; }

        public double Depth { get; set; }
        public double? Penetration { get; set; }
        public List<SawingMode> SawingModes { get; set; }

        public static void ConfigureParamsView(ParamsView view)
        {
            var sawingModesView = new SawingModesView();
            //view.BindingSource.DataSourceChanged += (s, e) => sawingModesView.sawingModesBindingSource.DataSource = view.GetParams<SawingTechOperation>().SawingModes;
            //view.AddTextBox(nameof(Thickness));
            view.AddCheckBox(nameof(IsExactlyBegin), "Начало точно");
            view.AddCheckBox(nameof(IsExactlyEnd), "Конец точно");
            view.AddTextBox(nameof(AngleA));
            view.AddTextBox(nameof(Departure));
            view.AddIndent();
            view.AddAcadObject(allowedTypes: $"{AcadObjectNames.Line},{AcadObjectNames.Arc},{AcadObjectNames.Lwpolyline}");
            view.AddTextBox(nameof(Depth));
            view.AddTextBox(nameof(Penetration));
            view.AddText("Режимы для криволинейных траекторий");
            view.AddControl(sawingModesView, 6);
        }

        public override void Execute(GeneralOperation generalOperation, Processor processor)
        {
            
        }
    }
}
