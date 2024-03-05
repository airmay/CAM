using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Dreambuild.AutoCAD;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public partial class ParamsView : UserControl
    {
        private const int ControsMargin = 1;

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
        private int RowHeight => (int)(Font.Height * 1.6); //24;
        private readonly Type _type;

        public object ParamsObject => BindingSource.DataSource;

        public T GetParams<T>() => BindingSource.GetSource<T>();

        private string GetDisplayName(string paramName) => _displayNames.TryGetValue(paramName, out var value) ? value : paramName;

        public ParamsView(Type type)
        {
            InitializeComponent();
            //tablePanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            _type = type;
            BindingSource.DataSource = type;

            type.GetMethod("ConfigureParamsView").Invoke(null, new[] { this });
        }

        public void ResetControls() => BindingSource.ResetBindings(false);

        private void AddControl(Control control)
        {
            control.Margin = new Padding(ControsMargin);
            tablePanel.Controls.Add(control);
        }

        private void AddRow(int height = 1)
        {
            tablePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, RowHeight * height));
            tablePanel.Height += RowHeight * height / 2;
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

        private Label AddLabelToParam(string paramName, string displayName, string toolTipText = null) => 
            AddLabel(displayName ?? GetDisplayName(paramName), toolTipText);

        private Label AddLabel(string displayName, string toolTipText = null)
        {
            var label = new Label
            {
                Text = displayName,
                Dock = DockStyle.Top,
                Height = RowHeight,
                Padding = new Padding(0, (int)(Font.Height * 0.1), 0, 0),
                TextAlign = ContentAlignment.TopLeft,
                AutoEllipsis = true,
                //BorderStyle = BorderStyle.FixedSingle
            };
            if (toolTipText != null)
                toolTip.SetToolTip(label, toolTipText);
            AddControl(label);
            
            return label;
        }

        #region AddParam

        public TextBox AddTextBox(string paramName, string displayName = null, bool readOnly = false, string toolTipText = null)
        {
            AddLabelToParam(paramName, displayName, toolTipText);

            var control = new TextBox
            {
                ReadOnly = readOnly,
                Dock = DockStyle.Fill,
            };
            control.DataBindings.Add(new Binding("Text", BindingSource, paramName, true,
                DataSourceUpdateMode.OnPropertyChanged, string.Empty));
            AddControl(control);

            return control;
        }

        public CheckBox AddCheckBox(string paramName, string displayName = null, string toolTipText = null)
        {
            AddLabelToParam(paramName, displayName, toolTipText);

            var control = new CheckBox
            {
                Height = RowHeight,
                // Dock = DockStyle.Fill
            };
            control.DataBindings.Add(new Binding("Checked", BindingSource, paramName, true));
            AddControl(control);

            return control;
        }

        public Label AddLabelText(string label, string text = "", string toolTipText = null)
        {
            AddLabel(label, toolTipText);
            var control = AddLabel(text);

            return control;
        }

        public Label AddText(string text, string toolTipText = null)
        {
            var label = AddLabel(text, toolTipText);
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

        public ComboBox AddComboBox(string displayName, object[] items, Action<int> selectedIndexChanged, string toolTipText = null)
        {
            var comboBox = AddComboBox(displayName, toolTipText);

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
            AddLabelToParam(paramName, displayName);

            var comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill,
                //Margin = new Padding(3, 3, 3, Font.Height / 2),
            };
            AddControl(comboBox);

            return comboBox;
        }

        public ComboBox AddMaterial() => AddComboBox<Material>("Material", "Материал");

        public ComboBox AddMachine(params MachineType[] values) => AddComboBox("MachineType", "Станок", values);

        #endregion

        #region AddSelectorParam

        public (TextBox, Button) CreateSelector(string displayName, string buttonText = "Ξ", string toolTipText = null)
        {
            AddLabel(displayName, toolTipText);

            var userControl = new UserControl
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(ControsMargin),
                //BorderStyle = BorderStyle.FixedSingle,
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
            var originInfo = _type.GetProperty(nameof(GeneralOperation.Origin));
            var originGroupInfo = _type.GetField(nameof(GeneralOperation.OriginGroup));

            var (textbox, button) = CreateSelector("Начало координат", "۞");

            BindingSource.DataSourceChanged += (s, e) => RefreshText();
            textbox.Enter += (s, e) =>
            {
                var originGroup = (ObjectId?)originInfo.GetValue(ParamsObject);
                if (originGroup.HasValue)
                    Acad.SelectObjectIds(originGroup.Value);
            };
            button.Click += (s, e) =>
            {
                Interaction.SetActiveDocFocus();
                var point = Interaction.GetPoint("\nВыберите точку начала координат");
                if (!point.IsNull())
                {
                    originInfo.SetValue(ParamsObject, point);
                    RefreshText();
                    if (originGroupInfo.GetValue(ParamsObject) != null)
                        ((ObjectId)originGroupInfo.GetValue(ParamsObject)).Delete();
                    originGroupInfo.SetValue(ParamsObject, Acad.CreateOriginObject(point));
                }
            };
            return this;

            void RefreshText()
            {
                var origin = (Point3d)originInfo.GetValue(ParamsObject);
                textbox.Text = $"{{{origin.X.Round(3)}, {origin.Y.Round(3)}}}";
            }
        }
        #endregion

        public ParamsView AddControl(Control control, int rows = 1, string propertyName = null, string dataMember = null)
        {
            control.Height = rows * RowHeight;
            tablePanel.SetColumnSpan(control, 2);
            tablePanel.Controls.Add(control);

            control.Dock = DockStyle.Fill;
            if (propertyName != null)
                control.DataBindings.Add(new Binding(propertyName, BindingSource, dataMember));

            return this;
        }
    }
}