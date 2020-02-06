using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.Tactile
{
    [ObjectView(typeof(BandsTechOperation))]
    public partial class BandsTechOperationView : UserControl, IObjectView
    {
        private BandsTechOperation _bandsTechOperation;

        public BandsTechOperationView()
        {
            InitializeComponent();
        }

        public void SetObject(object @object)
        {
            _bandsTechOperation = (BandsTechOperation)@object;
            bandsTechOperationBindingSource.DataSource = @object;
        }

        private void bCalculate_Click(object sender, EventArgs e)
        {
            _bandsTechOperation.CalcPassList();
            bandsTechOperationBindingSource.ResetBindings(false);
        }

        private void bUp_Click(object sender, EventArgs e)
        {
            if (_bandsTechOperation.PassList.SwapPrev(passListBindingSource.Position))
            {
                bandsTechOperationBindingSource.ResetBindings(false);
                passListBindingSource.Position--;
            }
        }

        private void bDown_Click(object sender, EventArgs e)
        {
            if (_bandsTechOperation.PassList.SwapNext(passListBindingSource.Position))
            {
                bandsTechOperationBindingSource.ResetBindings(false);
                passListBindingSource.Position++;
            }

        }
    }
}
