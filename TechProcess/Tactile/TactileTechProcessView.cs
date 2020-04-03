using Dreambuild.AutoCAD;
using System;
using System.Windows.Forms;

namespace CAM.Tactile
{
    public partial class TactileTechProcessView : UserControl, IDataView<TactileTechProcess>
    {
        private TactileTechProcess _techProcess;

        public TactileTechProcessView()
        {
            InitializeComponent();

            cbMachine.BindEnum(MachineType.Donatoni, MachineType.ScemaLogic, MachineType.Krea);
        }

        public void BindData(TactileTechProcess data)
        {
            tactileTechProcessBindingSource.DataSource = _techProcess = data;
            tactileTechProcessParamsBindingSource.DataSource = _techProcess.TactileTechProcessParams;
            tbTool.Text = _techProcess.Tool?.ToString();
            tbOrigin.Text = $"{{{_techProcess.OriginX}, {_techProcess.OriginY}}}";
            tbContour.Text = _techProcess.ProcessingArea?.GetDesc();
            tbObjects.Text = _techProcess.Objects?.GetDesc();
            SetParamsEnabled();
        }

        private void bTool_Click(object sender, EventArgs e)
        {
            if (!_techProcess.MachineType.CheckNotNull("Станок"))
                return;

            if (ToolService.SelectTool(_techProcess.MachineType.Value) is Tool tool)
            {
                _techProcess.Tool = tool;
                tbTool.Text = tool.ToString();
                _techProcess.Frequency = ToolService.CalcFrequency(tool, _techProcess.MachineType.Value, _techProcess.Material.Value);

                tactileTechProcessBindingSource.ResetBindings(false);
            }
        }

        private void bProcessingArea_Click(object sender, EventArgs e)
        {
            Interaction.SetActiveDocFocus();
            var ids = Interaction.GetSelection("\nВыберите объекты контура плитки", "LINE");
            if (ids.Length == 0)
                return;
            _techProcess.ProcessingArea = AcadObject.CreateList(ids);
            tbContour.Text = _techProcess.ProcessingArea.GetDesc();
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
                _techProcess.Objects = AcadObject.CreateList(ids);
                tbObjects.Text = _techProcess.Objects.GetDesc();
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
            Acad.SelectAcadObjects(_techProcess.ProcessingArea);
        }

        private void tbObjects_Enter(object sender, EventArgs e)
        {
            Acad.SelectAcadObjects(_techProcess.Objects);
        }

        private void tbOrigin_Enter(object sender, EventArgs e)
        {
            Acad.SelectObjectIds(_techProcess.OriginObject);
        }
    }
}
