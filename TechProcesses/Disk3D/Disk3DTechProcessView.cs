using Dreambuild.AutoCAD;
using System;
using System.Windows.Forms;

namespace CAM.Disk3D
{
    public partial class Disk3DTechProcessView : UserControl//, IDataView<Disk3DTechProcess>
    {
        public Disk3DTechProcess _techProcess;

        public Disk3DTechProcessView()
        {
            InitializeComponent();

            cbMachine.BindEnum(MachineType.Donatoni, MachineType.ScemaLogic);
            cbMaterial.BindEnum<Material>();
        }

        public void BindData(Disk3DTechProcess data)
        {
            disk3DTechProcessBindingSource.DataSource = _techProcess = data;
            tbTool.Text = _techProcess.Tool?.ToString();
            tbObjects.Text = _techProcess.ProcessingArea?.GetDesc();
            lbSize.Text = Acad.GetSize(_techProcess.ProcessingArea);
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

                disk3DTechProcessBindingSource.ResetBindings(false);
            }
        }

        private void bObjects_Click(object sender, EventArgs e)
        {
            Acad.SelectObjectIds();
            Interaction.SetActiveDocFocus();
            var ids = Interaction.GetSelection("\nВыберите объекты", $"{AcadObjectNames.Surface},{AcadObjectNames.Region}");
            if (ids.Length == 0)
                return;
            _techProcess.ProcessingArea = AcadObject.CreateList(ids);
            tbObjects.Text = _techProcess.ProcessingArea.GetDesc();
            lbSize.Text = Acad.GetSize(_techProcess.ProcessingArea);
        }

        private void tbObjects_Enter(object sender, EventArgs e)
        {
            Acad.SelectAcadObjects(_techProcess.ProcessingArea);
        }

        private void tbObjects_Leave(object sender, EventArgs e)
        {
            Acad.SelectObjectIds();
        }
    }
}
