using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CAM
{
    public static class ToolService
    {
        private static ToolsForm _toolsForm;

        public static Tool SelectTool(MachineType machineType)
        {
            if (_toolsForm == null)
            {
                _toolsForm = new ToolsForm();
                _toolsForm.LoadTools += LoadTools;
            }
            _toolsForm.Text = $"Инструмент {machineType}";
            _toolsForm.bLoad.Enabled = machineType == MachineType.ScemaLogic;
            _toolsForm.ToolBindingSource.DataSource = Settings.GetTools(machineType);

            var result = Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(_toolsForm);
            Settings.Save();

            return result == DialogResult.OK ? (Tool)_toolsForm.ToolBindingSource.Current : null;
        }

        public static int CalcFrequency(Tool tool, MachineType machineType, Material material)
        {
            // частота для диска 400мм - на граните 1900, на мраморе 2500
            // частота для диска 600мм - на граните 1500, на мраморе 2000
            var f400 = material == Material.Granite ? 1900 : 2500;
            var df = material == Material.Granite ? 1900 - 1500 : 2500 - 2000;
            var frequency = f400 - (tool.Diameter - 400) * df / 200;
            return Math.Min((int)frequency, Settings.GetMachineSettings(machineType).MaxFrequency);
        }

        public static bool Validate(Tool tool, ToolType toolType)
        {
            string message = null;
            if (tool == null)
                message = "Выберите инструмент";
            else if(tool.Type != toolType)
                message = $"Выберите инструмент типа {toolType.GetDescription()}";
            else
            {
                if (tool.Type == ToolType.Disk)
                {
                    if (tool.Diameter == 0)
                        message = $"Не указан диаметр инструмента";
                    if (tool.Thickness.GetValueOrDefault() == 0)
                        message = $"Не указана толщина инструмента";
                }
            }
            if (message != null)
                Acad.Alert(message);

            return message == null;
        }

        private static void LoadTools(object senser, EventArgs eventArgs)
        {
            string path = @"\\192.168.1.59\ssd\_CUST\Utensili.csv";
            if (!File.Exists(path))
            {
                Acad.Alert($"Не найден файл инструментов по адресу {path}");
                return;
            }

            var lines = File.ReadAllLines(path)
                .Select(p => Array.ConvertAll(p.Split(';'), k => new { result = double.TryParse(k, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value), value }))
                .Where(p => p.Length == 4 && p.All(k => k.result) && p[2].value == 0 && p[3].value == 1)
                .Select((p, ind) => new Tool
                {
                    Number = ind + 1,
                    Type = ToolType.Disk,
                    Diameter = p[0].value,
                    Thickness = p[1].value
                });
            var tools = Settings.GetTools(MachineType.ScemaLogic);
            //tools.Clear();
            //tools.AddRange(lines);
            _toolsForm.ToolBindingSource.DataSource = tools;
            _toolsForm.ToolBindingSource.ResetBindings(false);
        }
    }
}
