using System;
using System.Windows.Forms;

namespace CAM.Tactile
{
    [ObjectView(typeof(ConesTechOperation))]
    public partial class ConesTechOperationView : UserControl, IObjectView
    {
        private ConesTechOperation _conesTechOperation;

        public ConesTechOperationView()
        {
            InitializeComponent();
        }

        public void SetObject(object @object)
        {
            _conesTechOperation = (ConesTechOperation)@object;
            conesTechOperationBindingSource.DataSource = @object;
            tbTool.Text = _conesTechOperation.Tool.ToString();
        }

        private void bTool_Click(object sender, EventArgs e)
        {
            var tool = ToolsForm.Select(_conesTechOperation.TechProcess.MachineSettings.Tools, _conesTechOperation.TechProcess.MachineType);
            if (tool != null)
            {
                _conesTechOperation.Tool = tool;
                tbTool.Text = tool.ToString();
                if (_conesTechOperation.Frequency == 0)
                    _conesTechOperation.Frequency = Math.Min(tool.CalcFrequency(_conesTechOperation.TechProcess.Material), _conesTechOperation.TechProcess.MachineSettings.MaxFrequency);
                conesTechOperationBindingSource.ResetBindings(false);
            }
        }
    }
}
