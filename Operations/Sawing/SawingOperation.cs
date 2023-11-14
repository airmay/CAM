using CAM.TechProcesses.Sawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Operations.Sawing
{
    [Serializable]
    public class SawingOperation : Operation
    {
        public bool IsExactlyBegin { get; set; }

        public bool IsExactlyEnd { get; set; }

        public Side OuterSide { get; set; }

        public double AngleA { get; set; }

        public double Departure { get; set; }

        public List<SawingMode> SawingModes { get; set; }

        public static void ConfigureParamsView(ParamsView view)
        {
            var sawingModesView = new SawingModesView();
            //view.BindingSource.DataSourceChanged += (s, e) => sawingModesView.sawingModesBindingSource.DataSource = view.GetParams<SawingTechOperation>().SawingModes;

            view.AddCheckBox(nameof(IsExactlyBegin), "Начало точно");
            view.AddCheckBox(nameof(IsExactlyEnd), "Конец точно");
            view.AddTextBox(nameof(AngleA));
            view.AddTextBox(nameof(Departure));
            view.AddIndent();
            view.AddAcadObject(message: "Выберите объект",
                allowedTypes: $"{AcadObjectNames.Line},{AcadObjectNames.Arc},{AcadObjectNames.Lwpolyline}",
                afterSelect: ids =>
                {
                    var operation = view.GetParams<SawingTechOperation>();
                    operation.ProcessingArea = null;
                    var border = ((SawingTechProcess)operation.TechProcess).CreateExtraObjects(ids[0])[0];
                    operation.SetFromBorder(border);
                    view.ResetControls();
                    sawingModesView.sawingModesBindingSource.DataSource = operation.SawingModes;
                }
            );
            view.AddText("Режимы");
            view.AddControl(sawingModesView, 6);
        }

        public override void Execute(GeneralOperation generalOperation, Processor processor)
        {
            
        }
    }
}
