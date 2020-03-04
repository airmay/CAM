using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System.Windows.Forms;

namespace CAM.Tactile
{
    [ObjectView(typeof(MeasurementTechOperation))]
    public partial class MeasurementTechOperationView : UserControl, IObjectView
    {
        private MeasurementTechOperation _measurementTechOperation;

        public MeasurementTechOperationView()
        {
            InitializeComponent();
        }

        public void SetObject(object @object)
        {
            _measurementTechOperation = (MeasurementTechOperation)@object;
            tbPointsCount.Text = _measurementTechOperation.PointsX.Count.ToString();
        }

        private void bSelectPoints_Click(object sender, System.EventArgs e)
        {
            _measurementTechOperation.Clear();
            Interaction.SetActiveDocFocus();
            Point3d point;
            while (!(point = Interaction.GetPoint("\nВыберите точку измерения")).IsNull())
            {
                _measurementTechOperation.CreatePoint(point);
                tbPointsCount.Text = _measurementTechOperation.PointsX.Count.ToString();
            }
        }

        private void tbPointsCount_Enter(object sender, System.EventArgs e)
        {
            Acad.SelectObjectIds(_measurementTechOperation.PointObjectIds);
        }
    }
}
