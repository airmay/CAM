using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.TechOperation.Tactile
{
    [ParamsView(ProcessingType.Tactile)]
    public partial class TactileParamsView : ParamsView
    {
       // private TactileTechOperanion _tactileTechOperanion;

        public TactileParamsView()
        {
            InitializeComponent();
        }

        public override void SetParams(object @params)
        {
            //_tactileTechOperanion = (TactileTechOperanion)@params;
            //tactileParamsBindingSource.DataSource = _tactileTechOperanion.TactileParams;
            //tactileParamsBindingSource.ResetBindings(false);
            //tactileDefaultParamsView1.SetParams(_tactileTechOperanion.TactileParams);
        }

        private void bCalculate_Click(object sender, EventArgs e)
        {
            //_tactileTechOperanion.CalcPassList();
            tactileParamsBindingSource.ResetBindings(false);
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Acad.Alert("Некорректный числовой формат");           
        }
    }
}
