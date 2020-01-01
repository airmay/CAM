using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CAM.Domain;

namespace CAM.UI
{
    [DefaultParamsView(ProcessingType.Sawing)]
    public partial class SawingParamsDefaultView : ParamsView
    {
        private List<SawingMode>[] _sawingTechOperationParams;

        public SawingParamsDefaultView()
        {
            InitializeComponent();
        }

        public override void SetParams(object @params)
        {
            var sawingDefaultParams = (SawingDefaultParams)@params;
            _sawingTechOperationParams = new List<SawingMode>[] { sawingDefaultParams.SawingLineTechOperationParams.Modes, sawingDefaultParams.SawingCurveTechOperationParams.Modes };
            sawingModesView.sawingModesBindingSource.DataSource = _sawingTechOperationParams[0];
            cbTrajectoryType.SelectedIndex = 0;
        }

        private void cbTrajectoryType_SelectedIndexChanged(object sender, EventArgs e)
        {
            sawingModesView.sawingModesBindingSource.DataSource = _sawingTechOperationParams[cbTrajectoryType.SelectedIndex];
        }
    }
}
