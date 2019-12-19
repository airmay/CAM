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
        private CamDocument _camDocument;

        public ProgramView()
        {
            InitializeComponent();
        }

        public void SetCamDocument(CamDocument camDocument) => _camDocument = camDocument;

        //public void RefreshView() => textBox.Lines = _camDocument.GetProgramm();

        private void bSendProgram_Click(object sender, EventArgs e)
        {
            //_camDocument.SendProgram(textBox.Lines);
        }
    }
}
