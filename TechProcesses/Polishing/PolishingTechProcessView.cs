using Dreambuild.AutoCAD;
using System.Windows.Forms;

namespace CAM.TechProcesses.Polishing
{
    public partial class PolishingTechProcessView : UserControl, IDataView<PolishingTechProcess>
    {
        private PolishingTechProcess _techProcess;

        public PolishingTechProcessView()
        {
            InitializeComponent();
            cbMachine.BindEnum(MachineType.Donatoni, MachineType.Krea);
        }

        public void BindData(PolishingTechProcess data)
        {
            polishingTechProcessBindingSource.DataSource = _techProcess = data;
            tbOrigin.Text = $"{{{_techProcess.OriginX}, {_techProcess.OriginY}}}";
            tbObjects.Text = _techProcess.ProcessingArea?.GetDesc();
        }

        private void bOrigin_Click(object sender, System.EventArgs e)
        {
            Interaction.SetActiveDocFocus();
            var point = Interaction.GetPoint("\nВыберите точку начала координат");
            if (!point.IsNull())
            {
                _techProcess.OriginX = point.X.Round(3);
                _techProcess.OriginY = point.Y.Round(3);
                tbOrigin.Text = $"{{{_techProcess.OriginX}, {_techProcess.OriginY}}}";
                if (_techProcess.OriginObject != null)
                    Acad.DeleteObjects(_techProcess.OriginObject);
                _techProcess.OriginObject = Acad.CreateOriginObject(point);
            }
        }

        private void bObjects_Click(object sender, System.EventArgs e)
        {
            Interaction.SetActiveDocFocus();
            Acad.SelectObjectIds();
            var ids = Interaction.GetSelection("\nВыберите объекты контура");
            if (ids.Length == 0)
                return;
            _techProcess.ProcessingArea = AcadObject.CreateList(ids);
            tbObjects.Text = _techProcess.ProcessingArea.GetDesc();
            Acad.SelectObjectIds(ids);
        }
    }
}
