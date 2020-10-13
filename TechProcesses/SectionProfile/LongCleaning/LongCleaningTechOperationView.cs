using Autodesk.AutoCAD.DatabaseServices;
using Dreambuild.AutoCAD;
using System.Windows.Forms;

namespace CAM.TechProcesses.SectionProfile
{
    public partial class LongCleaningTechOperationView : UserControl//, IDataView<LongCleaningTechOperation>
    {
        private LongCleaningTechOperation _longCleaningTechOperation;

        public LongCleaningTechOperationView()
        {
            InitializeComponent();
        }

        public void BindData(LongCleaningTechOperation data)
        {
            longCleaningTechOperationBindingSource.DataSource = data;
            _longCleaningTechOperation = data;
            tbProfile.Text = _longCleaningTechOperation.Profile?.GetDesc();
        }

        private void bObjects_Click(object sender, System.EventArgs e)
        {
            Interaction.SetActiveDocFocus();
            Acad.SelectObjectIds();
            var ids = Interaction.GetSelection("\nВыберите профиль [Отрезок], [Дуга], [Полилиния]", $"{AcadObjectNames.Line},{AcadObjectNames.Arc},{AcadObjectNames.Lwpolyline}");
            if (ids.Length == 0)
                return;
            _longCleaningTechOperation.Profile = AcadObject.Create(ids[0]);
            tbProfile.Text = _longCleaningTechOperation.Profile.GetDesc();
            Acad.SelectObjectIds(ids);
        }
    }
}
