using Autodesk.AutoCAD.DatabaseServices;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAM.Sawing
{
    public partial class SawingTechProcessView : UserControl, IDataView<SawingTechProcess>
    {
        public SawingTechProcess _techProcess;

        public SawingTechProcessView()
        {
            InitializeComponent();

            cbMachine.BindEnum(MachineType.Donatoni, MachineType.ScemaLogic);
            cbMaterial.BindEnum<Material>();
        }

        public void BindData(SawingTechProcess data)
        {
            sawingTechProcessBindingSource.DataSource = _techProcess = data;
            tbTool.Text = _techProcess.Tool?.ToString();
            tbObjects.Text = _techProcess.ProcessingArea?.ToString();

            cbObjectType.SelectedIndex = 0;
            cbObjectType_SelectedIndexChanged(cbObjectType, EventArgs.Empty);
        }

        private void bTool_Click(object sender, EventArgs e)
        {
            if (_techProcess.MachineType.CheckIsNull("станок")) return;
            if (_techProcess.Material.CheckIsNull("материал")) return;

            var tool = ToolsForm.Select(_techProcess.MachineSettings.Tools, _techProcess.MachineType.Value);
            if (tool != null)
            {
                _techProcess.Tool = tool;
                tbTool.Text = tool.ToString();
                if (_techProcess.Frequency == 0)
                    _techProcess.Frequency = Math.Min(tool.CalcFrequency(_techProcess.Material.Value), _techProcess.MachineSettings.MaxFrequency);
                sawingTechProcessBindingSource.ResetBindings(false);
            }
        }

        private void bObjects_Click(object sender, EventArgs e)
        {
            Acad.DeleteExtraObjects();
            Acad.SelectObjectIds();
            Interaction.SetActiveDocFocus();
            var ids = Interaction.GetSelection("\nВыберите объекты распиловки", $"{AcadObjectNames.Line},{AcadObjectNames.Arc},{AcadObjectNames.Lwpolyline}");
            if (ids.Length == 0)
                return;
            _techProcess.ProcessingArea = new AcadObjectGroup(ids);
            tbObjects.Text = _techProcess.ProcessingArea.ToString();

            var curves = ids.QOpenForRead<Curve>().ToList();
            while (curves.Any())
            {
                var contour = new List<Curve>();
                var point = curves.SelectMany(p => p.GetStartEndPoints()).GroupBy(p => p).Where(p => p.Count() == 1).FirstOrDefault()?.Key ?? curves[0].StartPoint;
                while (curves.FirstOrDefault(p => p.HasPoint(point)) is Curve curve)
                {
                    contour.Add(curve);
                    curves.Remove(curve);
                    point = curve.NextPoint(point);
                }
                point = Interaction.GetLineEndPoint("Выберите направление внешней нормали к объекту", contour[0].StartPoint);
                var vector = contour[0].GetFirstDerivative(contour[0].StartParam);
                var sign = Graph.IsTurnRight(contour[0].StartPoint, contour[0].StartPoint + vector, point) ? -1 : 1;
                Graph.CreateHatch(contour, sign);
            }
        }

        private void tbObjects_Enter(object sender, EventArgs e)
        {
            Acad.SelectObjectIds(_techProcess.ProcessingArea?.ObjectIds);
        }

        private void cbObjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            sawingModesView.sawingModesBindingSource.DataSource = 
                cbObjectType.SelectedIndex == 0 ? _techProcess.SawingTechProcessParams.SawingLineModes : _techProcess.SawingTechProcessParams.SawingCurveModes;
        }
    }
}
