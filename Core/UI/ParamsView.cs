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
        private Dictionary<string, string> _displayNames = new Dictionary<string, string>
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
        private const int RowHeight = 24;
        private readonly Type _type;

        public object ParamsObject => BindingSource.DataSource;

        public T GetParams<T>() => BindingSource.GetSource<T>();

        private string GetDisplayName(string paramName) => _displayNames.TryGetValue(paramName, out var value) ? value : paramName;

        public ParamsView(Type type)
        {
            InitializeComponent();
            _type = type;
            BindingSource.DataSource = type;

            tablePanel.Height = 0;
            tablePanel.RowStyles.RemoveAt(0);

            type.GetMethod("ConfigureParamsView").Invoke(null, new[] { this });
        }

        public void ResetControls() => BindingSource.ResetBindings(false);

        private void AddRow(int height = 1)
        {
            tablePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, RowHeight * height));
            tablePanel.Height += RowHeight * height;
        }

        public ParamsView AddIndent()
        {
            AddRow();
            return this;
        }

        #region AddParam
        private void AddLabel(string displayName)
        {
            var label = new Label
            {
                Text = displayName,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };
            tablePanel.Controls.Add(label, 0, tablePanel.RowStyles.Count - 1);
        }

        public ParamsView AddParam(string paramName, string displayName = null, bool readOnly = false)
        {
            AddRow();
            AddLabel(displayName ?? GetDisplayName(paramName));

            Control control;
            var propType = _type.GetProperty(paramName).PropertyType;

            if (propType.UnderlyingSystemType == typeof(bool))
            {
                control = new CheckBox();
                control.DataBindings.Add(new Binding("Checked", BindingSource, paramName, true));
            }
            else
            {
                control = new TextBox
                {
                    ReadOnly = readOnly,
                    BackColor = readOnly ? SystemColors.Control : SystemColors.Window,
                    Dock = DockStyle.Fill
                };
                control.DataBindings.Add(new Binding("Text", BindingSource, paramName, true, DataSourceUpdateMode.OnPropertyChanged, string.Empty));
            }
            tablePanel.Controls.Add(control, 1, tablePanel.RowStyles.Count - 1);

            return this;
        }

        public ParamsView AddText(string displayName, Action<Label> labelProvider = null)
        {
            AddRow();
            AddLabel(displayName);

            var label = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            tablePanel.Controls.Add(label, 1, tablePanel.RowStyles.Count - 1);
            labelProvider?.Invoke(label);

            return this;
        }
        #endregion

        #region AddEnumParam

        public ParamsView AddEnumParam<T>(string paramName, string displayName = null, params T[] values) where T : struct
        {
            AddRow();
            AddLabel(displayName ?? GetDisplayName(paramName));

            var comboBox = new ComboBox();
            comboBox.BindEnum(values);
            comboBox.DataBindings.Add(new Binding("SelectedValue", BindingSource, paramName, true));
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.FormattingEnabled = true;
            comboBox.Dock = DockStyle.Fill;
            tablePanel.Controls.Add(comboBox, 1, tablePanel.RowStyles.Count - 1);

            return this;
        }

        public ParamsView AddMaterial() => AddEnumParam<Material>("Material", "Материал");

        public ParamsView AddMachine(params MachineType[] values) => AddEnumParam("MachineType", "Станок", values);

        #endregion

        #region AddSelectorParam

        public ParamsView AddSelector(string displayName, string buttonText, Action<TextBox, Button, BindingSource> configure)
        {
            var selector = CreateSelector(displayName, buttonText);
            configure?.Invoke((TextBox)selector.Controls[0] , (Button)selector.Controls[1], BindingSource);
            return this;
        }

        private UserControl CreateSelector(string displayName, string buttonText)
        {
            AddRow();
            AddLabel(displayName);

            var userControl = new UserControl
            {
                Margin = new Padding(3, 3, 3, 0),
                Size = new Size(124, RowHeight)
            };

            var textBox = new TextBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(0, 0),
                Size = new Size(100, 20),
                ReadOnly = true,
                BackColor = SystemColors.Control
            };
            userControl.Controls.Add(textBox);

            var button = new Button
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(102, 0),
                Size = new Size(22, 21),
                TabStop = false,
                Text = buttonText
            };
            userControl.Controls.Add(button);            

            userControl.Dock = DockStyle.Fill;
            tablePanel.Controls.Add(userControl, 1, tablePanel.RowStyles.Count - 1);

            return userControl;
        }

        public ParamsView AddTool()
        {
            var toolProp = _type.GetProperty("Tool");
            var machineProp = _type.GetProperty("MachineType");
            var materialProp = _type.GetProperty("Material");
            var frequencyProp = _type.GetProperty("Frequency");

            var selector = CreateSelector("Инструмент", "Ξ");

            var textBox = selector.Controls[0];
            BindingSource.DataSourceChanged += (s, e) => textBox.Text = toolProp.GetValue(ParamsObject)?.ToString();

            var button = selector.Controls[1];
            button.Click += (s, e) =>
            {
                var machine = (MachineType?)machineProp.GetValue(ParamsObject);
                var material = (Material?)materialProp.GetValue(ParamsObject);
                if (!machine.CheckNotNull("Станок") || !material.CheckNotNull("Материал"))
                    return;
                if (ToolService.SelectTool(machine.Value) is Tool tool)
                {
                    toolProp.SetValue(ParamsObject, tool);
                    textBox.Text = tool.ToString();
                    frequencyProp.SetValue(ParamsObject, ToolService.CalcFrequency(tool, machine.Value, material.Value));

                    BindingSource.ResetBindings(false);
                }
            };
            return this;
        }

        public ParamsView AddAcadObject(string paramName = "ProcessingArea", string displayName = null, string message = "Выбор объектов", string allowedTypes = "*", Action<ObjectId[]> afterSelect = null)
        {
            var prop = _type.GetProperty(paramName);

            var selector = CreateSelector(displayName ?? GetDisplayName(paramName), "۞");

            var textBox = selector.Controls[0];
            BindingSource.DataSourceChanged += prop.PropertyType == typeof(AcadObject) 
                ? new EventHandler((s, e) => textBox.Text = ((AcadObject)prop.GetValue(ParamsObject))?.GetDesc())
                : new EventHandler((s, e) => textBox.Text = ((List<AcadObject>)prop.GetValue(ParamsObject))?.GetDesc());
            textBox.Enter += prop.PropertyType == typeof(AcadObject)
                ? new EventHandler((s, e) => { if (prop.GetValue(ParamsObject) is AcadObject acadObj) Acad.SelectAcadObjects(new List<AcadObject> { acadObj }); })
                : new EventHandler((s, e) => { if (prop.GetValue(ParamsObject) is List<AcadObject> acadObjList) Acad.SelectAcadObjects(acadObjList); });

            var button = selector.Controls[1];
            button.Click += (s, e) =>
            {
                //textBox.Text = "";
                //prop.SetValue(ParamsObject, null);

                Interaction.SetActiveDocFocus();
                Acad.SelectObjectIds();
                var ids = Interaction.GetSelection($"\n{message}", allowedTypes);
                if (ids.Length == 0)
                    return;
                if (prop.PropertyType == typeof(AcadObject))
                {
                    var acadObject = AcadObject.Create(ids[0]);
                    prop.SetValue(ParamsObject, acadObject);
                    textBox.Text = acadObject.GetDesc();
                }
                else
                {
                    var acadOblects = AcadObject.CreateList(ids);
                    prop.SetValue(ParamsObject, acadOblects);
                    textBox.Text = acadOblects.GetDesc();
                }
                afterSelect?.Invoke(ids);
                Acad.SelectObjectIds(ids);
            };
            return this;
        }

        public ParamsView AddOrigin()
        {
            var originX = _type.GetProperty("OriginX");
            var originY = _type.GetProperty("OriginY");
            var originObject = _type.GetField(nameof(TechProcess.OriginObject));

            var selector = CreateSelector("Начало координат", "۞");

            var textBox = selector.Controls[0];
            BindingSource.DataSourceChanged += (s, e) => RefreshText();
            textBox.Enter += (s, e) => Acad.SelectObjectIds((ObjectId[])originObject?.GetValue(ParamsObject));

            var button = selector.Controls[1];
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

            void RefreshText() => textBox.Text = $"{{{originX.GetValue(ParamsObject)}, {originY.GetValue(ParamsObject)}}}";
        }
        #endregion

        public ParamsView AddControl(Control control, int height = 1)
        {
            AddRow(height);
            tablePanel.Controls.Add(control, 0, tablePanel.RowStyles.Count - 1);
            tablePanel.SetColumnSpan(control, 2);
            control.Dock = DockStyle.Fill;

            return this;
        }

        public ParamsView AddComboBox(string displayName, string[] items, Action<int> SelectedIndexChanged)
        {
            AddRow();
            AddLabel(displayName);

            var comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Dock = DockStyle.Fill
            };
            comboBox.Items.AddRange(items);
            comboBox.SelectedIndexChanged += new EventHandler((s, e) => SelectedIndexChanged(comboBox.SelectedIndex));

            BindingSource.DataSourceChanged += (s, e) =>
            {
                comboBox.SelectedIndex = 0;
                SelectedIndexChanged.Invoke(0);
            };
            tablePanel.Controls.Add(comboBox, 1, tablePanel.RowStyles.Count - 1);

            return this;
        }
    }
}