using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.UI
{
    public partial class TechProcessParamsView : UserControl
    {
        private SawingParamsDefaultView _sawingParamsDefaultView;

        public TechProcessParamsView()
        {
            InitializeComponent();
        }

        private void cbTechOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTechOperation.Text == "Распиловка")
            {
                if (_sawingParamsDefaultView == null)
                {
                    _sawingParamsDefaultView = new SawingParamsDefaultView();
                    gbTechOperationParams.Controls.Add(_sawingParamsDefaultView);
                    _sawingParamsDefaultView.Dock = DockStyle.Fill;
                }
                _sawingParamsDefaultView.BringToFront();
            }
        }
    }
}
