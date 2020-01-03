using System;
using System.Windows.Forms;

namespace CAM
{
	public partial class BorderProcessingAreaView : UserControl
	{
		public BorderProcessingAreaView()
		{
			InitializeComponent();
		}

        public void SetDataSource(BorderProcessingArea dataSource)
        {
            borderProcessingAreaBindingSource.DataSource = dataSource;
            borderProcessingAreaBindingSource.ResetBindings(false);
        }

        private void cbExactlyBegin_Click(object sender, EventArgs e)
        {
            cbAutoExactlyBegin.Checked = false;
        }

        private void cbExactlyEnd_Click(object sender, EventArgs e)
        {
            cbAutoExactlyEnd.Checked = false;
        }
    }
}
