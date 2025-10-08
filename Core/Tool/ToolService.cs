using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CAM
{
    public static class ToolService
    {
        private static ToolsForm _toolsForm;

        public static Tool SelectTool(Machine machine)
        {
            if (_toolsForm == null)
            {
                _toolsForm = new ToolsForm();
                _toolsForm.LoadTools += LoadScemaLogicTools;
                _toolsForm.SaveTools += (sender, args) => SaveTools(machine);
            }

            _toolsForm.Text = $"Инструмент {machine}";
            _toolsForm.ToolBindingSource.DataSource = GetTools(machine);
            _toolsForm.bLoad.Enabled = machine == Machine.ScemaLogic;

            var result = Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(_toolsForm);

            return result == DialogResult.OK ? (Tool)_toolsForm.ToolBindingSource.Current : null;
        }

        private const string ToolsFilePath =
#if DEBUG
            @"C:\Catalina\Tools";
#else
        @"\\192.168.1.230\public\Программы станок\CAM 2.0\Tools";
#endif
        private static string GetToolFileName(Machine machine) => Path.Combine(ToolsFilePath, $"{machine}.csv");

        private static readonly Dictionary<Machine, List<Tool>> Tools = new Dictionary<Machine, List<Tool>>();

        private static void SaveTools(Machine machine)
        {
            var file = GetToolFileName(machine);
            try
            {
                var lines = Tools[machine].Select(p => $"{p.Number};{p.Name};{(int)p.Type};{p.Diameter};{p.Thickness}");
                File.WriteAllLines(file, lines);
                Acad.Alert($"Сохранен файл инструментов {file}");
            }
            catch (Exception e)
            {
                Acad.Alert($"Ошибка при сохранении файла инструментов {file}", e);
            }
        }

        private static List<Tool> GetTools(Machine machine)
        {
            if (Tools.TryGetValue(machine, out var tools))
                return tools;

            var file = GetToolFileName(machine);
            if (File.Exists(file))
            {
                tools = File.ReadAllLines(file)
                    .Select(p => p.Split(';'))
                    .Select(p => new Tool
                    {
                        Number = ToInt(p[0]),
                        Name = p[1],
                        Type = (ToolType)ToInt(p[2]),
                        Diameter = ToDouble(p[3]),
                        Thickness = ToDouble(p[4]),
                    })
                    .ToList();
            }
            else
            {
                Acad.Alert($"Не найден файл инструментов: {file}");
                tools = new List<Tool>();
            }

            Tools.Add(machine, tools);

            return tools;
        }

        private static int ToInt(string s)
        {
            if (!int.TryParse(s, out var val))
                WriteErrorParsing(s);
            return val;
        }

        private static double ToDouble(string s)
        {
            if (!double.TryParse(s, out var val))
                WriteErrorParsing(s);
            return val;
        }

        private static void WriteErrorParsing(string par) =>
            Acad.Write($"Ошибка преобразования числового параметра {par}");

        private static void LoadScemaLogicTools(object senser, EventArgs eventArgs)
        {
            const string file = @"\\192.168.1.59\ssd\_CUST\Utensili.csv";
            if (!File.Exists(file))
            {
                Acad.Alert($"Не найден файл инструментов по адресу {file}");
                return;
            }

            var lines = File.ReadAllLines(file)
                .Select(p => Array.ConvertAll(p.Split(';'),
                    k => new
                    {
                        result = double.TryParse(k, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                            out var value),
                        value
                    }))
                .Where(p => p.Length == 4 && p.All(k => k.result) && p[2].value == 0 && p[3].value == 1)
                .Select((p, ind) => new Tool
                {
                    Number = ind + 1,
                    Type = ToolType.Disk,
                    Diameter = p[0].value,
                    Thickness = p[1].value
                });
            var tools = GetTools(Machine.ScemaLogic);
            tools.Clear();
            tools.AddRange(lines);
            _toolsForm.ToolBindingSource.DataSource = tools;
            _toolsForm.ToolBindingSource.ResetBindings(false);
        }

        public static int CalcFrequency(Tool tool, Machine machine, Material material)
        {
            // частота для диска 400мм - на граните 1900, на мраморе 2500
            // частота для диска 600мм - на граните 1500, на мраморе 2000
            var f400 = material == Material.Granite ? 1900 : 2500;
            var df = material == Material.Granite ? 1900 - 1500 : 2500 - 2000;
            var frequency = f400 - (tool.Diameter - 400) * df / 200;

            var maxFrequency = machine switch
            {
                Machine.Donatoni => 5000,
                Machine.Forma or Machine.Champion => 1200,
                _ => 10000
            };

            return Math.Min((int)frequency, maxFrequency);
        }

        public static bool Validate(Tool tool, ToolType toolType)
        {
            string message = null;
            if (tool == null)
                message = "Выберите инструмент";
            else if (tool.Type != toolType)
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
    }
}
