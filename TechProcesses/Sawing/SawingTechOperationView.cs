using Dreambuild.AutoCAD;
using System.Windows.Forms;

namespace CAM.Sawing
{
    public partial class SawingTechOperationView : UserControl, IDataView<SawingTechOperation>
    {
        private SawingTechOperation _techOperation;

        public SawingTechOperationView()
        {
            InitializeComponent();
        }

        public void BindData(SawingTechOperation data)
        {
            sawingTechOperationBindingSource.DataSource = _techOperation = data;
            tbObject.Text = _techOperation.ProcessingArea?.GetDesc();
            sawingModesView.sawingModesBindingSource.DataSource = _techOperation.SawingModes;
        }

        private void bObject_Click(object sender, System.EventArgs e)
        {
            Acad.SelectObjectIds();
            Interaction.SetActiveDocFocus();
            var ids = Interaction.GetSelection("\nВыберите объект", $"{AcadObjectNames.Line},{AcadObjectNames.Arc},{AcadObjectNames.Lwpolyline}");
            if (ids.Length == 0)
                return;
            Acad.DeleteExtraObjects();
            _techOperation.ProcessingArea = null;
            var border = ((SawingTechProcess)_techOperation.TechProcess).CreateExtraObjects(ids[0])[0];
            _techOperation.SetFromBorder(border);
            tbObject.Text = _techOperation.ProcessingArea.GetDesc();
            sawingTechOperationBindingSource.ResetBindings(false);
            sawingModesView.sawingModesBindingSource.DataSource = _techOperation.SawingModes;
        }
    }
}
