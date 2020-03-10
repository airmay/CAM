using System.Windows.Forms;

namespace CAM.Sawing
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
