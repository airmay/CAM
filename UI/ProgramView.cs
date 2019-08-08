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
    public partial class ProgramView : UserControl
    {
	    private CamManager _camManager;

        public ProgramView(CamManager camManager)
        {
            InitializeComponent();
            _camManager = camManager;
        }

        public void SetProgram(string[] lines) => textBox.Lines = lines;

        private void bSendProgram_Click(object sender, EventArgs e)
        {
            _camManager.SendProgram(textBox.Lines);
        }
    }
}
