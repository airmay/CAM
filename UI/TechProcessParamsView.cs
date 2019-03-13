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
    public partial class TechProcessParamsView : UserControl
    {
	    private readonly Dictionary<Type, Control> _paramsViews = new Dictionary<Type, Control>();

		public TechProcessParamsView()
        {
            InitializeComponent();

	        cbMachine.DataSource = Enum.GetValues(typeof(Machine));
			cbMaterial.DataSource = Enum.GetValues(typeof(MaterialType));
        }

		public void SetDataSource(TechProcessParams dataSource)
	    {
		    techProcessParamsBindingSource.DataSource = dataSource;
	    }

		private void cbTechOperation_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (cbTechOperation.Text)
			{
				case "Распиловка":
					GetParamsView<SawingParamsDefaultView>();
					break;
			}
        }

	    private T GetParamsView<T>() where T : Control, new()
	    {
		    if (!_paramsViews.TryGetValue(typeof(T), out var view))
		    {
			    view = new T { Dock = DockStyle.Fill };
			    _paramsViews.Add(typeof(T), view);
			    gbTechOperationParams.Controls.Add(view);
		    }
		    view.BringToFront();

		    return (T)view;
	    }
	}
}
