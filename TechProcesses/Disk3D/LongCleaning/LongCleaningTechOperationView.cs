using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.TechProcesses.Disk3D
{
    public partial class LongCleaningTechOperationView : UserControl, IDataView<LongCleaningTechOperation>
    {
        public LongCleaningTechOperationView()
        {
            InitializeComponent();
        }

        public void BindData(LongCleaningTechOperation data)
        {
            longCleaningTechOperationBindingSource.DataSource = data;
        }
    }
}
