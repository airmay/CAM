using System.Windows.Forms;

namespace CAM.Tactile
{
    [ObjectView(typeof(ChamfersTechOperation))]
    public partial class ChamfersTechOperationView : UserControl, IObjectView
    {
        private ChamfersTechOperation _chamfersTechOperation;

        public ChamfersTechOperationView()
        {
            InitializeComponent();
        }

        public void SetObject(object @object)
        {
            _chamfersTechOperation = (ChamfersTechOperation)@object;
            chamfersTechOperationBindingSource.DataSource = @object;
        }
    }
}
