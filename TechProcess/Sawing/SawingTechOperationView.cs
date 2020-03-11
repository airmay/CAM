using System.Windows.Forms;

namespace CAM.Sawing
{
    public partial class SawingTechOperationView : UserControl, IDataView<SawingTechOperation>
    {
        private SawingTechOperation _techOperation;

        public SawingTechOperationView()
        {
            InitializeComponent();
        }

        public void BindData(SawingTechOperation data)
        {
            sawingTechOperationBindingSource.DataSource = _techOperation = data;
            tbObject.Text = _techOperation.ProcessingArea?.ToString();
            sawingModesView.sawingModesBindingSource.DataSource = _techOperation.SawingModes;
        }
    }
}
