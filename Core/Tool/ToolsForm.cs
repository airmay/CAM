using System;
using System.Linq;
using System.Windows.Forms;

namespace CAM
{
    public partial class ToolsForm : Form
    {
        public ToolsForm()
        {
            InitializeComponent();

            Type.DisplayMember = "Description";
            Type.ValueMember = "Value";
            Type.DataSource = Enum.GetValues(typeof(ToolType))
                .Cast<Enum>()
                .Select(value => new
                {
                    Description = value.GetDescription(),
                    value
                })
                .OrderBy(item => item.value)
                .ToList();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            dataGridView.EndEdit();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
