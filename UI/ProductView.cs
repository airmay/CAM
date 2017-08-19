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
    public partial class ProductView : UserControl
    {
        public ProductView()
        {
            InitializeComponent();
        }

        public void SetData(List<Segment> segments)
        {
            segmentBindingSource.DataSource = segments;
        }

        private void bAddProdict_Click(object sender, EventArgs e)
        {
        }
    }
}
