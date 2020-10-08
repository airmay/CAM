using System.Windows.Forms;

namespace CAM.TechProcesses.SectionProfile
{
    public partial class LongCleaningTechOperationView : UserControl, IDataView<LongCleaningTechOperation>
    {
        public LongCleaningTechOperationView()
        {
            InitializeComponent();
        }

        public void BindData(LongCleaningTechOperation data)
        {
            longCleaningTechOperationBindingSource.DataSource = data;
        }
    }
}
