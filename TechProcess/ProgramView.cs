using System;
using System.Windows.Forms;

namespace CAM
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
