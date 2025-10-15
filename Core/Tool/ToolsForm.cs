using System;
using System.Windows.Forms;

namespace CAM
{
    public partial class ToolsForm : Form
    {
        public event EventHandler LoadTools;
        public event EventHandler SaveTools;

        public ToolsForm()
        {
            InitializeComponent();

            Type.DisplayMember = "Description";
            Type.ValueMember = "Value";
            Type.DataSource = Extensions.GetEnumValueDesc<ToolType>();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            dataGridView.EndEdit();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void bLoad_Click(object sender, EventArgs e)
        {
            LoadTools?.Invoke(this, EventArgs.Empty);
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            SaveTools?.Invoke(this, EventArgs.Empty);
        }
    }
}
