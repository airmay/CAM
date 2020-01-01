using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CAM.UI;

namespace CAM.Domain.Profiling
{
    [DefaultParamsView(ProcessingType.Profiling)]
    public partial class ProfilingParamsView : ParamsView
    {
        public ProfilingParamsView()
        {
            InitializeComponent();
        }
    }
}
