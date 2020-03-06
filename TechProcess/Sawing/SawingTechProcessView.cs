using System.Windows.Forms;

namespace CAM.Sawing
{
    public partial class SawingTechProcessView : UserControl, IDataView<SawingTechProcess>
    {
        public SawingTechProcess _techProcess;

        public SawingTechProcessView()
        {
            InitializeComponent();
        }

        public void BindData(SawingTechProcess data)
        {
            _techProcess = data;
        }
    }
}
