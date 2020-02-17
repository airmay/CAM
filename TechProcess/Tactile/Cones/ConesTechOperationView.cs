using System;
using System.Windows.Forms;

namespace CAM.Tactile
{
    [ObjectView(typeof(ConesTechOperation))]
    public partial class ConesTechOperationView : UserControl, IObjectView
    {
        private ConesTechOperation _conesTechOperation;

        public ConesTechOperationView()
        {
            InitializeComponent();
        }

        public void SetObject(object @object)
        {
            _conesTechOperation = (ConesTechOperation)@object;
            conesTechOperationBindingSource.DataSource = @object;
        }
    }
}
