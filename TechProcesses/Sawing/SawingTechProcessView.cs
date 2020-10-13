using Dreambuild.AutoCAD;
using System;
using System.Windows.Forms;

namespace CAM.Sawing
{
    public partial class SawingTechProcessView : UserControl//, IDataView<SawingTechProcess>
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
            tbObjects.Text = _techProcess.ProcessingArea?.GetDesc();

            cbObjectType.SelectedIndex = 0;
            cbObjectType_SelectedIndexChanged(cbObjectType, EventArgs.Empty);
        }

        private void bTool_Click(object sender, EventArgs e)
        {
            if (!_techProcess.MachineType.CheckNotNull("Станок") || !_techProcess.Material.CheckNotNull("Материал"))
                return;

            if (ToolService.SelectTool(_techProcess.MachineType.Value) is Tool tool)
            {
                _techProcess.Tool = tool;
                tbTool.Text = tool.ToString();
                _techProcess.Frequency = ToolService.CalcFrequency(tool, _techProcess.MachineType.Value, _techProcess.Material.Value);
                   
                sawingTechProcessBindingSource.ResetBindings(false);
            }
        }

        private void bObjects_Click(object sender, EventArgs e)
        {
            Acad.SelectObjectIds();
            Interaction.SetActiveDocFocus();
            var ids = Interaction.GetSelection("\nВыберите объекты распиловки", $"{AcadObjectNames.Line},{AcadObjectNames.Arc},{AcadObjectNames.Lwpolyline}");
            if (ids.Length == 0)
                return;
            Acad.DeleteExtraObjects();
            _techProcess.CreateExtraObjects(ids);
            tbObjects.Text = _techProcess.ProcessingArea.GetDesc();            
        }

        private void tbObjects_Enter(object sender, EventArgs e)
        {
            Acad.SelectAcadObjects(_techProcess.ProcessingArea);
        }

        private void cbObjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            sawingModesView.sawingModesBindingSource.DataSource = 
                cbObjectType.SelectedIndex == 0 ? _techProcess.SawingTechProcessParams.SawingLineModes : _techProcess.SawingTechProcessParams.SawingCurveModes;
        }
    }
}
