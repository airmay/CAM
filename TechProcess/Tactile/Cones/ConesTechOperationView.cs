using System.Windows.Forms;

namespace CAM.Tactile
{
    public partial class ConesTechOperationView : UserControl, IDataView<ConesTechOperation>
    {
        public ConesTechOperationView()
        {
            InitializeComponent();
        }

        public void BindData(ConesTechOperation data)
        {
            conesTechOperationBindingSource.DataSource = data;
        }
    }
}
