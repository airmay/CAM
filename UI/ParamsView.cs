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
    public partial class ParamsView : UserControl
    {
        public ParamsView()
        {
            InitializeComponent();
        }

        public virtual void SetParams(object @params)
        {
        }
    }
}
