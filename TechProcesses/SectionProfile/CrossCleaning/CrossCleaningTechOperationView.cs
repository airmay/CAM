using System.Windows.Forms;

namespace CAM.TechProcesses.SectionProfile
{
    public partial class CrossCleaningTechOperationView : UserControl//, IDataView<CrossCleaningTechOperation>
    {
        public CrossCleaningTechOperationView()
        {
            InitializeComponent();
        }

        public void BindData(CrossCleaningTechOperation data)
        {
            crossCleaningTechOperationBindingSource.DataSource = data;
        }
    }
}
