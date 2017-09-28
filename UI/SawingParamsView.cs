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
    public partial class SawingParamsView : UserControl
    {
        public SawingParamsView()
        {
            InitializeComponent();
        }

        private void gvSawingModes_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Введите корректное числовое значение", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void cbCalcExactlyBegin_CheckedChanged(object sender, EventArgs e)
        {
            cbExactlyBegin.Enabled = !cbCalcExactlyBegin.Checked;
        }

        private void cbCalcExactlyEnd_CheckedChanged(object sender, EventArgs e)
        {
            cbExactlyEnd.Enabled = !cbCalcExactlyEnd.Checked;
        }
    }
}
