using System;
using System.Windows.Forms;
using Dreambuild.AutoCAD;

namespace CAM.TechProcesses.Drilling
{
    public partial class DrillingTechProcessView :  UserControl, IDataView<DrillingTechProcess>
    {
        private DrillingTechProcess _techProcess;

        public DrillingTechProcessView()
        {
            InitializeComponent();
        }

        public void BindData(DrillingTechProcess data)
        {
            drillingTechProcessBindingSource.DataSource = _techProcess = data;
            tbOrigin.Text = $"{{{_techProcess.OriginX}, {_techProcess.OriginY}}}";
            tbObjects.Text = _techProcess.ProcessingArea?.GetDesc();
            _techProcess.MachineType = MachineType.Krea;
        }

        private void bOrigin_Click(object sender, EventArgs e)
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

        private void bObjects_Click(object sender, EventArgs e)
        {
            Interaction.SetActiveDocFocus();
            Acad.SelectObjectIds();
            var ids = Interaction.GetSelection("\nВыберите окружности", AcadObjectNames.Circle);
            if (ids.Length == 0)
                return;
            _techProcess.ProcessingArea = AcadObject.CreateList(ids);
            tbObjects.Text = _techProcess.ProcessingArea.GetDesc();
            Acad.SelectObjectIds(ids);
        }
    }
}
