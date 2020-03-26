﻿using System.Collections.Generic;
using System.Windows.Forms;

namespace CAM
{
    public static class ToolService
    {
        private static ToolsForm _toolsForm;
        private static Dictionary<MachineType, List<Tool>> _tools = new Dictionary<MachineType, List<Tool>>();

        public static void AddMachineTools(MachineType machineType, List<Tool> tools) => _tools.Add(machineType, tools);

        public static Tool Select(MachineType machineType)
        {
            if (_toolsForm == null)
                _toolsForm = new ToolsForm();
            _toolsForm.Text = $"Инструмент {machineType}";
            _toolsForm.ToolBindingSource.DataSource = _tools[machineType];

            if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(_toolsForm) == DialogResult.OK)
                return (Tool)_toolsForm.ToolBindingSource.Current;
            return null;
        }
    }
}
