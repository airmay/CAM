using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Dreambuild.AutoCAD;

namespace CAM.TechProcesses.SectionProfile
{
    public partial class SectionProfileTechProcessView : UserControl, IDataView<SectionProfileTechProcess>
    {
        private SectionProfileTechProcess _techProcess;

        public SectionProfileTechProcessView()
        {
            InitializeComponent();
            cbMachine.BindEnum(MachineType.Donatoni, MachineType.ScemaLogic);
            cbMaterial.BindEnum<Material>();
        }

        public void BindData(SectionProfileTechProcess data)
        {
            sectionProfileTechProcessBindingSource.DataSource = _techProcess = data;
            tbTool.Text = _techProcess.Tool?.ToString();
            tbObjects.Text = _techProcess.ProcessingArea?.GetDesc();
            tbRail.Text = _techProcess.Rail?.GetDesc();
        }

        private void bTool_Click(object sender, EventArgs e)
        {
            if (!_techProcess.Material.CheckNotNull("Материал"))
                return;

            if (ToolService.SelectTool(_techProcess.MachineType.Value) is Tool tool)
            {
                _techProcess.Tool = tool;
                tbTool.Text = tool.ToString();
                _techProcess.Frequency = ToolService.CalcFrequency(tool, _techProcess.MachineType.Value, _techProcess.Material.Value);

                sectionProfileTechProcessBindingSource.ResetBindings(false);
            }
        }

        private void bObjects_Click(object sender, EventArgs e)
        {
            Interaction.SetActiveDocFocus();
            Acad.SelectObjectIds();
            var ids = Interaction.GetSelection("\nВыберите профиль [Отрезок], [Дуга], [Полилиния]", $"{AcadObjectNames.Line},{AcadObjectNames.Arc},{AcadObjectNames.Lwpolyline}");
            if (ids.Length == 0)
                return;
            ids = new ObjectId[] { ids[0] };
            _techProcess.ProcessingArea = AcadObject.CreateList(ids);
            tbObjects.Text = _techProcess.ProcessingArea.GetDesc();
            Acad.SelectObjectIds(ids);
        }

        private void bRail_Click(object sender, EventArgs e)
        {
            Interaction.SetActiveDocFocus();
            Acad.SelectObjectIds();
            var ids = Interaction.GetSelection("\nВыберите направляющую [Отрезок]", AcadObjectNames.Line);
            if (ids.Length == 1)
            {
                _techProcess.Rail = AcadObject.Create(ids[0]);
                tbRail.Text = _techProcess.Rail.GetDesc();
                Acad.SelectObjectIds(ids);
            }
            else
            {
                _techProcess.Rail = null;
                tbRail.Text = "";
            }
        }

        private void tbObjects_Enter(object sender, EventArgs e)
        {
            Acad.SelectAcadObjects(_techProcess.ProcessingArea);
        }

        private void tbRail_Enter(object sender, EventArgs e)
        {
            Acad.SelectAcadObjects(new List<AcadObject> { _techProcess.Rail });
        }
    }
}
