using System.Windows.Forms;

namespace CAM.Tactile
{
    public partial class ChamfersTechOperationView : UserControl//, IDataView<ChamfersTechOperation>
    {
        public ChamfersTechOperationView()
        {
            InitializeComponent();
        }

        public void BindData(ChamfersTechOperation data)
        {
            chamfersTechOperationBindingSource.DataSource = data;
        }
    }
}
