using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CAM.Domain;

namespace CAM.UI
{
	public partial class BorderProcessingAreaView : UserControl
	{
		public BorderProcessingAreaView()
		{
			InitializeComponent();
		}

		public void SetDataSource(BorderProcessingArea dataSource)
		{
			borderProcessingAreaBindingSource.DataSource = dataSource;
		}
	}
}
