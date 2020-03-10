using System;
using System.Windows.Forms;

namespace CAM.Sawing
{
    public partial class SawingTechProcessView : UserControl, IDataView<SawingTechProcess>
    {
        public SawingTechProcess _techProcess;

        public SawingTechProcessView()
        {
            InitializeComponent();

            cbMaterial.BindEnum<Material>();
            cbMaterial.SelectedIndex = -1;

            cbObjectType.SelectedIndex = 0;
        }

        public void BindData(SawingTechProcess data)
        {
            _techProcess = data;
        }

        private void bTool_Click(object sender, System.EventArgs e)
        {
            var tool = ToolsForm.Select(_techProcess.MachineSettings.Tools, _techProcess.MachineType);
            if (tool != null)
            {
                _techProcess.Tool = tool;
                tbTool.Text = tool.ToString();
                if (_techProcess.Frequency == 0)
                    _techProcess.Frequency = Math.Min(tool.CalcFrequency(_techProcess.Material), _techProcess.MachineSettings.MaxFrequency);
                //tactileTechProcessBindingSource.ResetBindings(false);
            }
        }
    }
}
