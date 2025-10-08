using Autodesk.AutoCAD.DatabaseServices;
using CAM.CncWorkCenter;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CAM
{
    public class ParamsControl : UserControl
    {
        private string GetDisplayName(string paramName) => _displayNames.TryGetValue(paramName, out var value) ? value : paramName;

        private readonly Dictionary<string, string> _displayNames = new()
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

        private TableLayoutPanel _tablePanel;
        private BindingSource _bindingSource;
        private ToolTip _toolTip;
        private readonly Type _type;
        public T GetData<T>() => (T)_bindingSource.DataSource;

        public ParamsControl(Type type)
        {
            InitializeComponent();
            _type = type;
            _bindingSource.DataSource = type;

            type.GetMethod("ConfigureParamsView")?.Invoke(null, [this]);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            _bindingSource = new BindingSource();
            _toolTip = new ToolTip();
            _tablePanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                ColumnCount = 2,
                //CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };
            _tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            _tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            Controls.Add(_tablePanel);

            ResumeLayout(false);
        }

        public void SetData(object data) => _bindingSource.DataSource = data;

        public void ResetControls() => _bindingSource.ResetBindings(false);

        private int RowHeight => (int)(Font.Height * 1.8);

        private void AddRow(int height = 1)
        {
            _tablePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, RowHeight * height));
            _tablePanel.Height += RowHeight * height / 2;
        }

        private void AddRowH(int height)
        {
            var rowStyle = new RowStyle(SizeType.Absolute, height + Font.Size * 2);
            //tablePanel.RowStyles.Add(rowStyle);
            _tablePanel.Height += (int)rowStyle.Height;
        }

        private void AddControl(Control control, string hint, string paramName = null, string propertyName = null)
        {
            if (paramName != null)
                control.DataBindings.Add(propertyName, _bindingSource, paramName, true);

            control.Margin = new Padding(1);
            if (hint != null)
                _toolTip.SetToolTip(control, hint);
            _tablePanel.Controls.Add(control);
        }

        public Panel AddIndent()
        {
            var panel = new Panel
            {
                Height = (int)(Font.Height / 1.5)
            };
            _tablePanel.SetColumnSpan(panel, 2);
            _tablePanel.Controls.Add(panel);

            return panel;
        }

        private Label AddLabel(string displayName, string hint = null)
        {
            var label = new Label
            {
                Text = GetDisplayName(displayName),
                Dock = DockStyle.Top,
                //Height = RowHeight,
                TextAlign = ContentAlignment.MiddleRight,
                AutoEllipsis = true,
                //BorderStyle = BorderStyle.FixedSingle
            };
            AddControl(label, hint);

            return label;
        }

        public Label AddLabelText(string label, string text = "", string hint = null)
        {
            AddLabel(label, hint);
            var control = AddLabel(text);

            return control;
        }

        public Label AddText(string text, string hint = null)
        {
            var label = AddLabel(text, hint);
            label.TextAlign = ContentAlignment.MiddleLeft;
            _tablePanel.SetColumnSpan(label, 2);

            return label;
        }

        public TextBox AddTextBox(string paramName, string displayName = null, bool readOnly = false, string hint = null)
        {
            AddLabel(displayName ?? paramName, hint);

            var control = new TextBox
            {
                ReadOnly = readOnly,
                Dock = DockStyle.Top,
            };
            AddControl(control, hint, paramName, "Text");

            return control;
        }


        public CheckBox AddCheckBox(string paramName, string displayName = null, string hint = null)
        {
            AddLabel(displayName ?? paramName, hint);

            var control = new CheckBox();
            AddControl(control, hint, paramName, "Checked");

            return control;
        }

        #region AddComboBox

        public ComboBox AddComboBox<T>(string paramName, string displayName = null, params T[] values) where T : struct
        {
            var comboBox = AddComboBox(paramName, displayName);
            comboBox.BindEnum(values);

            return comboBox;
        }

        public ComboBox AddComboBox(string displayName, object[] items, Action<int> selectedIndexChanged, string hint = null)
        {
            var comboBox = AddComboBox(displayName, hint);

            comboBox.Items.AddRange(items);
            comboBox.SelectedIndexChanged += (s, e) => selectedIndexChanged(comboBox.SelectedIndex);
            _bindingSource.DataSourceChanged += (s, e) =>
            {
                comboBox.SelectedIndex = 0;
                selectedIndexChanged.Invoke(0);
            };

            return comboBox;
        }

        private ComboBox AddComboBox(string paramName, string displayName = null, string hint = null)
        {
            AddLabel(displayName ?? paramName, hint);

            var comboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList, 
            };
            AddControl(comboBox, hint, paramName, "SelectedValue");

            return comboBox;
        }

        public ComboBox AddMaterial() => AddComboBox<Material>("Material", "Материал");

        public ComboBox AddMachine(params Machine[] values) => AddComboBox("Machine", "Станок", values);

        #endregion

        #region AddSelectorParam

        public (TextBox, Button) CreateSelector(string paramName, string displayName, string buttonText = "Ξ", string hint = null)
        {
            AddLabel(displayName ?? paramName, hint);

            var tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.ColumnCount = 2;
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 25F));
            tableLayout.Height = 0;
            tableLayout.Margin = new Padding(0);

            var textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Margin = new Padding(1),
            };
            if (hint != null)
                _toolTip.SetToolTip(textBox, hint);
            textBox.DataBindings.Add("Text", _bindingSource, $"{paramName}.Description");
            tableLayout.Controls.Add(textBox);

            var button = new Button
            {
                Dock = DockStyle.Fill,
                Text = buttonText,
                Margin = new Padding(0),
                TabStop = false
           };
            tableLayout.Controls.Add(button);
            _tablePanel.Controls.Add(tableLayout);

            return (textBox, button);
        }

        public (TextBox textbox, Button button) AddTool()
        {
            var (textbox, button) = CreateSelector("Tool","Инструмент");

            button.Click += (s, e) =>
            {
                var techProcess = GetData<ProcessingCnc>();
                if (!techProcess.Machine.CheckNotNull("Станок") || !techProcess.Material.CheckNotNull("Материал"))
                    return;

                if (ToolService.SelectTool(techProcess.Machine!.Value) is { } tool)
                {
                    techProcess.Tool = tool;
                    techProcess.Frequency = ToolService.CalcFrequency(tool, techProcess.Machine.Value, techProcess.Material!.Value);
                    _bindingSource.ResetBindings(false);
                }
            };
            return (textbox, button);
        }

        public (TextBox textbox, Button button) AddAcadObject(string paramName = "ProcessingArea",
            string displayName = null, string message = "Выбор объектов", string allowedTypes = "*",
            Action<ObjectId[]> afterSelect = null, string hint = "Обрабатываемые объекты")
        {
            var (textbox, button) = CreateSelector(paramName, displayName, "۞", hint);

            var prop = _type.GetProperty(paramName);
            textbox.Enter += (s, e) =>
                Acad.SelectObjectIds(prop.GetValue(_bindingSource.DataSource).As<AcadObject>()?.ObjectIds);
            button.Click += (s, e) =>
            {
                Interaction.SetActiveDocFocus();
                Acad.SelectObjectIds();
                var ids = Interaction.GetSelection($"\n{message}", allowedTypes);
                if (ids.Length == 0)
                    return;

                var acadObject = AcadObject.Create(ids);
                prop.SetValue(_bindingSource.DataSource, acadObject);
                textbox.Text = acadObject.ToString();

                afterSelect?.Invoke(ids);
                Acad.SelectObjectIds(ids);
            };

            return (textbox, button);
        }

        public (TextBox textbox, Button button) AddOrigin()
        {
            var (textbox, button) = CreateSelector("Origin", "Начало координат", "۞");
            
            textbox.Enter += (s, e) => Acad.SelectObjectIds(GetData<IProcessing>().Origin.OriginObject?.ObjectIds);
            button.Click += (s, e) =>
            {
                GetData<IProcessing>().Origin.CreateOriginObject();
                textbox.DataBindings[0].ReadValue();
            };

            return (textbox, button);
        }
        #endregion

        public Control AddControl(Control control, int rows = 1, string propertyName = null, string dataMember = null)
        {
            control.Height = rows * RowHeight;
            _tablePanel.SetColumnSpan(control, 2);
            _tablePanel.Controls.Add(control);

            control.Dock = DockStyle.Fill;
            if (propertyName != null)
                control.DataBindings.Add(propertyName, _bindingSource, dataMember);

            return control;
        }
    }
}