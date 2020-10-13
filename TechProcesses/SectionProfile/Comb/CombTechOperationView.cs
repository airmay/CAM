using Dreambuild.AutoCAD;
using System.Windows.Forms;

namespace CAM.TechProcesses.SectionProfile
{
    public partial class CombTechOperationView : UserControl //, IDataView<CombTechOperation>
    {
        private CombTechOperation _combTechOperation;
        public CombTechOperationView()
        {
            InitializeComponent();
        }

        public void BindData(CombTechOperation data)
        {
            bindingSource.DataSource = data;
            _combTechOperation = data;
            tbProfile.Text = _combTechOperation.Profile?.GetDesc();
        }

        private void bObjects_Click(object sender, System.EventArgs e)
        {
            Interaction.SetActiveDocFocus();
            Acad.SelectObjectIds();
            var ids = Interaction.GetSelection("\nВыберите профиль [Полилиния]", $"{AcadObjectNames.Lwpolyline}");
            if (ids.Length == 0)
                return;
            _combTechOperation.Profile = AcadObject.Create(ids[0]);
            tbProfile.Text = _combTechOperation.Profile.GetDesc();
            Acad.SelectObjectIds(ids);
        }
    }
}
