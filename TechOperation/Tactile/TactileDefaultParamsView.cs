using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.TechOperation.Tactile
{
    [DefaultParamsView(ProcessingType.Tactile)]
    public partial class TactileDefaultParamsView : ParamsView
    {
        public TactileDefaultParamsView()
        {
            InitializeComponent();
        }

        public override void SetParams(object @params)
        {
            tactileDefaultParamsBindingSource.DataSource = @params;
        }
    }
}
