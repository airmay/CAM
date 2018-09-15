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
    public partial class ProgramView : UserControl
    {
        public ProgramView()
        {
            InitializeComponent();
        }

        public void ShowProgram(List<ProgramLine> program)
        {
            dataGridView.DataSource = program;
        }
    }
}
