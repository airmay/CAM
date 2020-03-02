using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System.Collections.Generic;
using System.Linq;
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
            tbPointsCount.Text = _measurementTechOperation.PointsX?.Length.ToString();
            //conesTechOperationBindingSource.DataSource = @object;
        }

        private void bSelectPoints_Click(object sender, System.EventArgs e)
        {
            Interaction.SetActiveDocFocus();
            var points = new List<Point3d>();
            Point3d point;
            while (!(point = Interaction.GetPoint("\nВыберите точку измерения")).IsNull())
            {
                points.Add(point);
                tbPointsCount.Text = points.Count.ToString();
                Acad.CreateMeasurementPoint(point);
            }
            if (points.Any())
            {
                _measurementTechOperation.PointsX = points.Select(p => p.X).ToArray();
                _measurementTechOperation.PointsY = points.Select(p => p.Y).ToArray();
            }
        }
    }
}
