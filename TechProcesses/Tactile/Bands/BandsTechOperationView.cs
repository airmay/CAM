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
    public partial class BandsTechOperationView : UserControl, IDataView<BandsTechOperation>
    {
        private BandsTechOperation _techOperation;

        public BandsTechOperationView()
        {
            InitializeComponent();
        }

        public void BindData(BandsTechOperation data)
        {
            bandsTechOperationBindingSource.DataSource = _techOperation = data;
        }

        private void bCalculate_Click(object sender, EventArgs e)
        {
            _techOperation.CalcPassList();
            bandsTechOperationBindingSource.ResetBindings(false);
        }

        private void bUp_Click(object sender, EventArgs e)
        {
            if (_techOperation.PassList.SwapPrev(passListBindingSource.Position))
            {
                bandsTechOperationBindingSource.ResetBindings(false);
                passListBindingSource.Position--;
            }
        }

        private void bDown_Click(object sender, EventArgs e)
        {
            if (_techOperation.PassList.SwapNext(passListBindingSource.Position))
            {
                bandsTechOperationBindingSource.ResetBindings(false);
                passListBindingSource.Position++;
            }

        }
    }
}
