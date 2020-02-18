using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAM.Tactile
{
    [ObjectView(typeof(TactileTechProcess))]
    public partial class TactileTechProcessView : UserControl, IObjectView
    {
        private TactileTechProcess _tactileTechProcess;

        public TactileTechProcessView()
        {
            InitializeComponent();

            cbMachine.DisplayMember = "Description";
            cbMachine.ValueMember = "Value";
            cbMachine.DataSource = new List<MachineType>() { MachineType.ScemaLogic, MachineType.Donatoni }
                .ConvertAll(value => new
                {
                    Description = value.ToString(),
                    value
                });
        }

        public void SetObject(object @object)
        {
            _tactileTechProcess = (TactileTechProcess)@object;
            tactileTechProcessBindingSource.DataSource = @object;
            tactileTechProcessParamsBindingSource.DataSource = _tactileTechProcess.TactileTechProcessParams;
            tbTool.Text = _tactileTechProcess.Tool?.ToString();
            tbOrigin.Text = $"{_tactileTechProcess.OriginX},{_tactileTechProcess.OriginY}";
            tbContour.Text = _tactileTechProcess.ProcessingArea?.AcadObjectIds.GetDesc();
            SetParamsEnabled();
        }

        private void bTool_Click(object sender, EventArgs e)
        {
            var tool = ToolsForm.Select(_tactileTechProcess.MachineSettings.Tools, _tactileTechProcess.MachineType);
            if (tool != null)
            {
                _tactileTechProcess.Tool = tool;
                tbTool.Text = tool.ToString();
                if (_tactileTechProcess.Frequency == 0)
                    _tactileTechProcess.Frequency = Math.Min(tool.CalcFrequency(_tactileTechProcess.Material), _tactileTechProcess.MachineSettings.MaxFrequency);
                tactileTechProcessBindingSource.ResetBindings(false);
            }
        }

        private void bProcessingArea_Click(object sender, EventArgs e)
        {
            Interaction.SetActiveDocFocus();
            var ids = Interaction.GetSelection("\nВыберите объекты контура плитки", "LINE");
            if (ids.Length == 0)
                return;
            _tactileTechProcess.CalcContour(ids);
            _tactileTechProcess.ProcessingArea = new ProcessingArea(ids);
            tbContour.Text = _tactileTechProcess.ProcessingArea.AcadObjectIds.GetDesc();
            Acad.SelectObjectIds(ids);
            SetParamsEnabled();
        }

        private void bOrigin_Click(object sender, EventArgs e)
        {
            Interaction.SetActiveDocFocus();
            var origin = Interaction.GetPoint("\nВыберите точку начала координат");
            if (!origin.IsNull())
            {
                _tactileTechProcess.OriginX = (int)origin.X;
                _tactileTechProcess.OriginY = (int)origin.Y;
                tbOrigin.Text = $"{_tactileTechProcess.OriginX},{_tactileTechProcess.OriginY}";
            }
        }

        private void bObjects_Click(object sender, EventArgs e)
        {
            Interaction.SetActiveDocFocus();
            var ids = Interaction.GetSelection("\nВыберите 2 элемента плитки");
            if (ids.Length > 0)
            {
                _tactileTechProcess.CalcType(ids);
                tbObjects.Text = ids.GetDesc();
                tactileTechProcessBindingSource.ResetBindings(false);
                SetParamsEnabled();
            }
        }

        private void SetParamsEnabled()
        {
            var enabled = _tactileTechProcess.Contour != null &&  _tactileTechProcess.Type != null;
            tbBandWidth.Enabled = enabled;
            tbBandSpacing.Enabled = enabled;
            tbBandStart1.Enabled = enabled;
            tbBandStart2.Enabled = enabled;
        }
    }
}
