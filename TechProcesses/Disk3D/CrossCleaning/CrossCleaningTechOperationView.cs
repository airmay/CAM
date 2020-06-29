using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.TechProcesses.Disk3D.CrossCleaning
{
    public partial class CrossCleaningTechOperationView : UserControl, IDataView<CrossCleaningTechOperation>
    {
        public CrossCleaningTechOperationView()
        {
            InitializeComponent();
        }

        public void BindData(CrossCleaningTechOperation data)
        {
            crossCleaningTechOperationBindingSource.DataSource = data;
        }
    }
}
