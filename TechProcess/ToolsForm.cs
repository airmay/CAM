using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAM
{
    public partial class ToolsForm : Form
    {
        private static ToolsForm _instance;

        public static Tool Select(List<Tool> tools)
        {
            if (_instance == null)
                _instance = new ToolsForm();
            _instance.toolBindingSource.DataSource = tools;
            if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(_instance) == DialogResult.OK)
                return (Tool)_instance.toolBindingSource.Current;
            return null;
        }


        public ToolsForm()
        {
            InitializeComponent();

            Type.DisplayMember = "Description";
            Type.ValueMember = "Value";
            Type.DataSource = Enum.GetValues(typeof(ToolType))
                .Cast<Enum>()
                .Select(value => new
                {
                    Description = value.GetDescription(),
                    value
                })
                .OrderBy(item => item.value)
                .ToList();
        }
    }
}
