using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Dreambuild.AutoCAD;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM
{
    public partial class ParamsView : UserControl
    {
        private readonly Dictionary<string, string> _displayNames = new Dictionary<string, string>
        {
            ["Frequency"] = "Шпиндель",
            ["Feed"] = "Подача",
            ["CuttingFeed"] = "Подача",
            ["PenetrationFeed"] = "Скор.малая",
            ["TransitionFeed"] = "Переход",

            ["ProcessingArea"] = "Объекты",
            ["Rail"] = "Направляющая",
            ["Profile"] = "Профиль",

            ["Step"] = "Шаг",
            ["StepX"] = "Шаг по X",
            ["StepY"] = "Шаг по Y",
            ["StepPass"] = "Шаг межстрочный",
            ["StepLong"] = "Шаг продольный",
            ["StartPass"] = "Первый проход",
            ["EndPass"] = "Последний проход",

            ["Thickness"] = "Толщина",
            ["Depth"] = "Глубина",
            ["Penetration"] = "Заглубление",
            ["Departure"] = "Выезд",
            ["IsUplifting"] = "Подъем",
            ["Delta"] = "Припуск",
            ["ZMax"] = "Z максимум",
            ["ZSafety"] = "Z безопасности",
            ["ZEntry"] = "Z входа",
            ["AngleA"] = "Угол вертикальный",
        };
        private int RowHeight => (int)(Font.Height * 1.8); //24;
        private readonly Type _type;

        public object ParamsObject => BindingSource.DataSource;

        public T GetParams<T>() => BindingSource.GetSource<T>();

        private string GetDisplayName(string paramName) => _displayNames.TryGetValue(paramName, out var value) ? value : paramName;

        public ParamsView(Type type)
        {
            InitializeComponent();
            _type = type;
            BindingSource.DataSource = type;

            type.GetMethod("ConfigureParamsView").Invoke(null, new[] { this });
        }

        public void ResetControls() => BindingSource.ResetBindings(false);

        private void AddRow(int height = 1)
        {
            tablePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, RowHeight * height));
            tablePanel.Height += RowHeight * height;
        }
        
        private void AddRowH(int height)
        {
            var rowStyle = new RowStyle(SizeType.Absolute, height + Font.Size * 2);
            //tablePanel.RowStyles.Add(rowStyle);
            tablePanel.Height += (int)rowStyle.Height;
        }

        public Panel AddIndent()
        {
            var panel = new Panel
            {
                Height = (int)(Font.Height / 1.5)
            };
            tablePanel.SetColumnSpan(panel, 2);
            tablePanel.Controls.Add(panel);

            return panel;
        }

        private Label AddLabel(string paramName, string displayName) => AddLabel(displayName ?? GetDisplayName(paramName));

        private Label AddLabel(string displayName)
        {
            var label = new Label
            {
                Text = displayName,
                Dock = DockStyle.Top,
                Height = (int)(Font.Height * 1.8),
                Padding = new Padding(0, (int)(Font.Height * 0.1), 0, 0),
                TextAlign = ContentAlignment.TopLeft,
                AutoEllipsis = true,
            };
            tablePanel.Controls.Add(label);

            return label;
        }

        #region AddParam

        public TextBox AddTextBox(string paramName, string displayName = null, bool readOnly = false)
        {
            AddLabel(paramName, displayName);

            var control = new TextBox
            {
                ReadOnly = readOnly,
                Dock = DockStyle.Fill,
            };
            control.DataBindings.Add(new Binding("Text", BindingSource, paramName, true,
                DataSourceUpdateMode.OnPropertyChanged, string.Empty));
            tablePanel.Controls.Add(control);

            return control;
        }

        public CheckBox AddCheckBox(string paramName, string displayName = null)
        {
            AddLabel(paramName, displayName);

            var control = new CheckBox
            {
                Height = (int)(Font.Height * 1.4)
            };
            control.DataBindings.Add(new Binding("Checked", BindingSource, paramName, true));
            tablePanel.Controls.Add(control);

            return control;
        }

        public Label AddLabelText(string label, string text = "")
        {
            AddLabel(label);
            var control = AddLabel(text);

            return control;
        }

        public Label AddText(string text)
        {
            var label = AddLabel(text);
            tablePanel.SetColumnSpan(label, 2);

            return label;
        }
        #endregion

        #region AddComboBox

        public ComboBox AddComboBox<T>(string paramName, string displayName = null, params T[] values) where T : struct
        {
            var comboBox = AddComboBox(paramName, displayName);

            comboBox.BindEnum(values);
            comboBox.DataBindings.Add(new Binding("SelectedValue", BindingSource, paramName, true));

            return comboBox;
        }

        public ComboBox AddComboBox(string displayName, object[] items, Action<int> selectedIndexChanged)
        {
            var comboBox = AddComboBox(displayName);

            comboBox.Items.AddRange(items);
            comboBox.SelectedIndexChanged += new EventHandler((s, e) => selectedIndexChanged(comboBox.SelectedIndex));
            BindingSource.DataSourceChanged += (s, e) =>
            {
                comboBox.SelectedIndex = 0;
                selectedIndexChanged.Invoke(0);
            };

            return comboBox;
        }

        private ComboBox AddComboBox(string paramName, string displayName = null)
        {
            AddLabel(paramName, displayName);

            var comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Top,
                Margin = new Padding(3, 3, 3, Font.Height / 2),
            };
            tablePanel.Controls.Add(comboBox);

            return comboBox;
        }

        public ComboBox AddMaterial() => AddComboBox<Material>("Material", "Материал");

        public ComboBox AddMachine(params MachineType[] values) => AddComboBox("MachineType", "Станок", values);

        #endregion

        #region AddSelectorParam

        public (TextBox, Button) CreateSelector(string displayName, string buttonText = "Ξ")
        {
            AddLabel(displayName);

            var userControl = new UserControl
            {
                Dock = DockStyle.Fill,
                Height = 0
            };

            var textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true
            };
            userControl.Controls.Add(textBox);

            var button = new Button
            {
                Dock = DockStyle.Right,
                Width = (int)(textBox.Height * 1.5),
                TabStop = false,
                Text = buttonText,
            };
            userControl.Controls.Add(button);
            tablePanel.Controls.Add(userControl);

            return (textBox, button);
        }

        public (TextBox textbox, Button button) AddTool()
        {
            var toolProp = _type.GetProperty("Tool");
            var machineProp = _type.GetProperty("MachineType");
            var materialProp = _type.GetProperty("Material");
            var frequencyProp = _type.GetProperty("Frequency");

            var (textbox, button) = CreateSelector("Инструмент");
            BindingSource.DataSourceChanged += (s, e) => textbox.Text = toolProp.GetValue(ParamsObject)?.ToString();
            button.Click += (s, e) =>
            {
                var machine = (MachineType?)machineProp.GetValue(ParamsObject);
                var material = (Material?)materialProp.GetValue(ParamsObject);
                if (!machine.CheckNotNull("Станок") || !material.CheckNotNull("Материал"))
                    return;
                if (ToolService.SelectTool(machine.Value) is Tool tool)
                {
                    toolProp.SetValue(ParamsObject, tool);
                    textbox.Text = tool.ToString();
                    frequencyProp.SetValue(ParamsObject, ToolService.CalcFrequency(tool, machine.Value, material.Value));

                    BindingSource.ResetBindings(false);
                }
            };
            return (textbox, button);
        }

        public (TextBox textbox, Button button) AddAcadObject(string paramName = "ProcessingArea", string displayName = null, string message = "Выбор объектов", string allowedTypes = "*", Action<ObjectId[]> afterSelect = null)
        {
            var prop = _type.GetProperty(paramName);

            var (textbox, button) = CreateSelector(displayName ?? GetDisplayName(paramName), "۞");

            BindingSource.DataSourceChanged +=
                (s, e) => textbox.Text = ((AcadObject)prop.GetValue(ParamsObject))?.ToString();
            textbox.Enter += (s, e) => 
            {
                if (prop.GetValue(ParamsObject) is AcadObject acadObj)
                    Acad.SelectObjectIds(acadObj.ObjectIds);
            };
            button.Click += (s, e) =>
            {
                Interaction.SetActiveDocFocus();
                Acad.SelectObjectIds();
                var ids = Interaction.GetSelection($"\n{message}", allowedTypes);
                if (ids.Length == 0)
                    return;

                var acadObject = AcadObject.Create(ids);
                prop.SetValue(ParamsObject, acadObject);
                textbox.Text = acadObject.ToString();

                afterSelect?.Invoke(ids);
                Acad.SelectObjectIds(ids);
            };
            return (textbox, button);
        }

        public ParamsView AddOrigin()
        {
            var originX = _type.GetProperty("OriginX");
            var originY = _type.GetProperty("OriginY");
            var originObject = _type.GetField(nameof(MillingTechProcess.OriginObject));

            var (textbox, button) = CreateSelector("Начало координат", "۞");

            BindingSource.DataSourceChanged += (s, e) => RefreshText();
            textbox.Enter += (s, e) => Acad.SelectObjectIds((ObjectId[])originObject?.GetValue(ParamsObject));
            button.Click += (s, e) =>
            {
                Interaction.SetActiveDocFocus();
                var point = Interaction.GetPoint("\nВыберите точку начала координат");
                if (!point.IsNull())
                {
                    originX.SetValue(ParamsObject, point.X.Round(3));
                    originY.SetValue(ParamsObject, point.Y.Round(3));
                    RefreshText();
                    if (originObject.GetValue(ParamsObject) != null)
                        Acad.DeleteObjects((ObjectId[])originObject.GetValue(ParamsObject));
                    originObject.SetValue(ParamsObject, Acad.CreateOriginObject(point));
                }
            };
            return this;

            void RefreshText() => textbox.Text = $"{{{originX.GetValue(ParamsObject)}, {originY.GetValue(ParamsObject)}}}";
        }
        #endregion

        public ParamsView AddControl(Control control, int height = 1)
        {
            control.Height = height * Font.Height;
            tablePanel.SetColumnSpan(control, 2);
            tablePanel.Controls.Add(control);

            //AddRow(height);
            //tablePanel.Controls.Add(control, 0, tablePanel.RowStyles.Count - 1);
            //tablePanel.SetColumnSpan(control, 2);
            control.Dock = DockStyle.Fill;

            return this;
        }
    }
}