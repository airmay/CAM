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
        private TechProcess _techProcess;

        public TechProcessParamsView()
        {
            InitializeComponent();

	        cbMachine.DataSource = Enum.GetValues(typeof(MachineType));
			cbMaterial.DataSource = Enum.GetValues(typeof(Material));
        }

		public void SetTechProcess(TechProcess techProcess)
	    {
            _techProcess = techProcess;
            techProcessParamsBindingSource.DataSource = techProcess.TechProcessParams;

            cbTechOperation.Text = null;
            foreach (Control control in gbTechOperationParams.Controls)
                if (control is UserControl)
                    control.Hide();
        }

        private void cbTechOperation_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (cbTechOperation.Text)
			{
				case "Распиловка":
					var view = GetParamsView<SawingParamsDefaultView>();
                    var factory = (SawingTechOperationFactory)_techProcess.GetFactory(TechOperationType.Sawing);
                    view.SetFactory(factory);

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
            view.Visible = true;
            view.BringToFront();

		    return (T)view;
	    }

        private void edToolNumber_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !_techProcess.SetTool(edToolNumber.Text);
            if (e.Cancel)
            {
                MessageBox.Show($"Инструмент '{edToolNumber.Text}' не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //edToolNumber.SelectAll();
                //edToolNumber.Focus();
            }
        }
    }
}
