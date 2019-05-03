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
		
	    public void SetDataSource(SawingTechOperationParams dataSource)
	    {
		    sawingParamsBindingSource.DataSource = dataSource;
            sawingParamsBindingSource.ResetBindings(false);
            sawingModesView.sawingModesBindingSource.DataSource = ((SawingTechOperationParams)sawingParamsBindingSource.DataSource).Modes;
	    }
	}
}
