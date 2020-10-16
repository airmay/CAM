using System.Windows.Forms;

namespace CAM.Disk3D
{
    public partial class CombTechOperationView : UserControl//, IDataView<CombTechOperation>
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
