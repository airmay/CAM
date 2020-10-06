using System.Windows.Forms;

namespace CAM.TechProcesses.SectionProfile
{
    public partial class CombTechOperationView : UserControl, IDataView<CombTechOperation>
    {
        public CombTechOperationView()
        {
            InitializeComponent();
        }

        public void BindData(CombTechOperation data)
        {
            bindingSource.DataSource = data;
        }
    }
}
