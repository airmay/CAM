using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CAM.Tactile
{
    public partial class TactileTechProcessView : UserControl, IDataView<TactileTechProcess>
    {
        private TactileTechProcess _techProcess;

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

        public void BindData(TactileTechProcess data)
        {
            tactileTechProcessBindingSource.DataSource = _techProcess = data;
            tactileTechProcessParamsBindingSource.DataSource = _techProcess.TactileTechProcessParams;
            tbTool.Text = _techProcess.Tool?.ToString();
            tbOrigin.Text = $"{{{_techProcess.OriginX}, {_techProcess.OriginY}}}";
            tbContour.Text = _techProcess.ProcessingArea?.ToString();
            tbObjects.Text = _techProcess.Objects?.ToString();
            SetParamsEnabled();
        }

        private void bTool_Click(object sender, EventArgs e)
        {
            var tool = ToolsForm.Select(_techProcess.MachineSettings.Tools, _techProcess.MachineType);
            if (tool != null)
            {
                _techProcess.Tool = tool;
                tbTool.Text = tool.ToString();
                if (_techProcess.Frequency == 0)
                    _techProcess.Frequency = Math.Min(tool.CalcFrequency(_techProcess.Material), _techProcess.MachineSettings.MaxFrequency);
                tactileTechProcessBindingSource.ResetBindings(false);
            }
        }

        private void bProcessingArea_Click(object sender, EventArgs e)
        {
            Interaction.SetActiveDocFocus();
            var ids = Interaction.GetSelection("\nВыберите объекты контура плитки", "LINE");
            if (ids.Length == 0)
                return;
            _techProcess.ProcessingArea = new AcadObjects(ids);
            tbContour.Text = _techProcess.ProcessingArea.ToString();
            Acad.SelectObjectIds(ids);
            SetParamsEnabled();
        }

        private void bOrigin_Click(object sender, EventArgs e)
        {
            Interaction.SetActiveDocFocus();
            var point = Interaction.GetPoint("\nВыберите точку начала координат");
            if (!point.IsNull())
            {
                _techProcess.OriginX = point.X.Round(3);
                _techProcess.OriginY = point.Y.Round(3);
                tbOrigin.Text = $"{{{_techProcess.OriginX}, {_techProcess.OriginY}}}";
                if (_techProcess.OriginObject != null)
                    Acad.DeleteObjects(_techProcess.OriginObject);
                _techProcess.OriginObject = Acad.CreateOriginObject(point);
            }
        }

        private void bObjects_Click(object sender, EventArgs e)
        {
            if (_techProcess.ProcessingArea == null)
            {
                Acad.Alert("Укажите контур плитки");
                return;
            }
            Interaction.SetActiveDocFocus();
            var ids = Interaction.GetSelection("\nВыберите 2 элемента плитки");
            if (ids.Length > 0)
            {
                _techProcess.CalcType(ids);
                _techProcess.Objects = new AcadObjects(ids);
                tbObjects.Text = _techProcess.Objects.ToString();
                tactileTechProcessBindingSource.ResetBindings(false);
                SetParamsEnabled();
            }
        }

        private void SetParamsEnabled()
        {
            var enabled = _techProcess.ProcessingArea != null &&  _techProcess.Type != null;
            tbBandWidth.Enabled = enabled;
            tbBandSpacing.Enabled = enabled;
            tbBandStart1.Enabled = enabled;
            tbBandStart2.Enabled = enabled;
        }

        private void tbContour_Enter(object sender, EventArgs e)
        {
            Acad.SelectObjectIds(_techProcess.ProcessingArea?.ObjectIds);
        }

        private void tbObjects_Enter(object sender, EventArgs e)
        {
            Acad.SelectObjectIds(_techProcess.Objects?.ObjectIds);
        }

        private void tbOrigin_Enter(object sender, EventArgs e)
        {
            Acad.SelectObjectIds(_techProcess.OriginObject);
        }
    }
}
