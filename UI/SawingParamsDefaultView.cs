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
    public partial class SawingParamsDefaultView : UserControl
    {
        private List<SawingMode>[] _sawingTechOperationParams;

        public SawingParamsDefaultView()
        {
            InitializeComponent();
        }

        public void SetParams(object factory)
        {
            _sawingTechOperationParams = new List<SawingMode>[] { factory.SawingLineTechOperationParams.Modes, factory.SawingCurveTechOperationParams.Modes };
            sawingModesView.sawingModesBindingSource.DataSource = _sawingTechOperationParams[0];
            cbTrajectoryType.SelectedIndex = 0;
        }

        private void cbTrajectoryType_SelectedIndexChanged(object sender, EventArgs e)
        {
            sawingModesView.sawingModesBindingSource.DataSource = _sawingTechOperationParams[cbTrajectoryType.SelectedIndex];
        }
    }
}
