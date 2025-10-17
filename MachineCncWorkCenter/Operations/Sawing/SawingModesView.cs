using System.Windows.Forms;

namespace CAM.MachineCncWorkCenter.Operations.Sawing
{
    public partial class SawingModesView : UserControl
    {
        public SawingModesView()
        {
            InitializeComponent();
        }

        public object DataSource
        {
            get => sawingModesBindingSource.DataSource;
            set => sawingModesBindingSource.DataSource = value;
        }

        private void gvSawingModes_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Введите корректное числовое значение", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
