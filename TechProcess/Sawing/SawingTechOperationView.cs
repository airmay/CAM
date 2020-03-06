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
            _techOperation = data;
        }
    }
}
