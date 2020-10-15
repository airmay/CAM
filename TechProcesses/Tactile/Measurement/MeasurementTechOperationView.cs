using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System.Windows.Forms;

namespace CAM.Tactile
{
    public partial class MeasurementTechOperationView : UserControl//, IDataView<MeasurementTechOperation>
    {
        private MeasurementTechOperation _techOperation;

        public MeasurementTechOperationView()
        {
            InitializeComponent();
        }
       
        public void BindData(MeasurementTechOperation data)
        {
            _techOperation = data;
            tbPointsCount.Text = _techOperation.PointsX.Count.ToString();
            tactileTechProcessBindingSource.DataSource = data.TechProcess;
        }

        private void bSelectPoints_Click(object sender, System.EventArgs e)
        {
            _techOperation.Clear();
            Interaction.SetActiveDocFocus();
            Point3d point;
            while (!(point = Interaction.GetPoint("\nВыберите точку измерения")).IsNull())
            {
                _techOperation.CreatePoint(point);
                tbPointsCount.Text = _techOperation.PointsX.Count.ToString();
            }
        }

        private void tbPointsCount_Enter(object sender, System.EventArgs e)
        {
            Acad.SelectObjectIds(_techOperation.PointObjectIds);
        }

        private void rbCalcMethodChanged(object sender, System.EventArgs e)
        {
            if (rbAverage.Checked)
                _techOperation.CalcMethod = MeasurementTechOperation.CalcMethodType.Average;
            else if(rbMinimum.Checked)
                _techOperation.CalcMethod = MeasurementTechOperation.CalcMethodType.Minimum;
            else
                _techOperation.CalcMethod = MeasurementTechOperation.CalcMethodType.Сorners;
        }
    }
}
