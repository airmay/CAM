using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using CAM.TechOperation;

namespace CAM
{
    public partial class TechProcessParamsView : UserControl
    {
        private TechProcess _techProcess;
        private Dictionary<MachineType, List<ProcessingAttribute>> _processingTypes;

        public TechProcessParamsView()
        {
            InitializeComponent();

            cbMachine.DataSource = Enum.GetValues(typeof(MachineType));
            cbMaterial.DataSource = Enum.GetValues(typeof(Material));

            cbTechOperation.DisplayMember = "Name";
            cbTechOperation.ValueMember = "Type";

            _processingTypes = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(p => p.IsClass && typeof(ITechOperationFactory).IsAssignableFrom(p) && Attribute.IsDefined(p, typeof(ProcessingAttribute)))
                    .Select(p => Attribute.GetCustomAttribute(p, typeof(ProcessingAttribute)) as ProcessingAttribute)
                    .GroupBy(p => p.Machine)
                    .ToDictionary(p => p.Key, p => p.ToList());
            _processingTypes.Add(MachineType.Krea, null); //TODO temp fix
        }

        public void SetTechProcess(TechProcess techProcess)
        {
            _techProcess = techProcess;
            techProcessParamsBindingSource.DataSource = techProcess.TechProcessParams;

            cbTechOperation.DataSource = _processingTypes[techProcess.TechProcessParams.Machine];
            cbTechOperation.DisplayMember = "Name";
            cbTechOperation.SelectedItem = _processingTypes[techProcess.TechProcessParams.Machine]?.FirstOrDefault(p => p.Type == techProcess.ProcessingType);
        }

        private void edToolNumber_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !_techProcess.SetTool(edToolNumber.Text);
            if (e.Cancel)
            {
                Acad.Alert($"Инструмент '{edToolNumber.Text}' не найден");
                //edToolNumber.SelectAll();
                //edToolNumber.Focus();
            }
        }

        private void cbTechOperation_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var processingType = (ProcessingType)cbTechOperation.SelectedValue;
            _techProcess.SetProcessingType(processingType);
        }

        private void cbMachine_SelectionChangeCommitted(object sender, EventArgs e)
        {
            cbTechOperation.DataSource = _processingTypes[(MachineType)cbMachine.SelectedItem];
            cbTechOperation.DisplayMember = "Name";
            cbTechOperation.SelectedItem = null;
            _techProcess.SetProcessingType(null);
        }

        private ParamsForm _paramsForm;

        private void bTechOperationParams_Click(object sender, EventArgs e)
        {
            if (_techProcess.ProcessingType != null)
            {
                if (_paramsForm == null)
                    _paramsForm = new ParamsForm();
                _paramsForm.SetParams(_techProcess.ProcessingType.Value, _techProcess.TechOperationParams, cbTechOperation.Text);
                Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(_paramsForm);
            }
        }

        private List<Tool> _tools = new List<Tool> { new Tool { Diameter = 400, Thickness = 7, Number = 1, Type = ToolType.Disk }, new Tool { Diameter = 100, Number = 2, Type = ToolType.Mill } };

        private void button1_Click(object sender, EventArgs e)
        {
            var tool = ToolsForm.Select(_tools);
            if (tool != null)
                edToolNumber.Text = tool.ToString();
        }
    }
}
