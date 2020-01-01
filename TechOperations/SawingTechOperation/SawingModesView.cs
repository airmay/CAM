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
    public partial class SawingModesView : UserControl
    {
        public SawingModesView()
        {
            InitializeComponent();
        }

        private void gvSawingModes_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Введите корректное числовое значение", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
