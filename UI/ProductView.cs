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

        public void SetData(List<Product> products)
        {
            productBindingSource.DataSource = products;
        }

        private void bAddProdict_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Введите число");
        }

        private void cbProduct_Validating(object sender, CancelEventArgs e)
        {
            MessageBox.Show("cbProduct_Validating");
        }

        private void cbProduct_Validated(object sender, EventArgs e)
        {
            MessageBox.Show("cbProduct_Validated");
        }
    }
}
