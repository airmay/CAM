using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CAM.Domain;

namespace CAM.UI
{
    public partial class SawingParamsView : UserControl
    {
        public SawingParamsView()
        {
            InitializeComponent();
        }

        private void sawingParamsBindingSource_DataSourceChanged(object sender, EventArgs e)
        {
            sawingModesView.sawingModesBindingSource.DataSource = ((SawingTechOperationParams)sawingParamsBindingSource.DataSource).Modes;
        }
    }
}
