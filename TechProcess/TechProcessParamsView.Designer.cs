namespace CAM
{
    partial class TechProcessParamsView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cbTechOperation = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbMachine = new System.Windows.Forms.ComboBox();
            this.techProcessParamsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cbMaterial = new System.Windows.Forms.ComboBox();
            this.lbMaterial = new System.Windows.Forms.Label();
            this.lbMachine = new System.Windows.Forms.Label();
            this.gbTool = new System.Windows.Forms.GroupBox();
            this.edThickness = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.edDiameter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.edToolNumber = new System.Windows.Forms.TextBox();
            this.lbToolNumber = new System.Windows.Forms.Label();
            this.gbBillet = new System.Windows.Forms.GroupBox();
            this.edBilletThickness = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.edFrequency = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.edZSafety = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.edPenetrationRate = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.pParams = new System.Windows.Forms.Panel();
            this.bTechOperationParams = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.techProcessParamsBindingSource)).BeginInit();
            this.gbTool.SuspendLayout();
            this.gbBillet.SuspendLayout();
            this.pParams.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbTechOperation
            // 
            this.cbTechOperation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTechOperation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTechOperation.FormattingEnabled = true;
            this.cbTechOperation.Location = new System.Drawing.Point(94, 30);
            this.cbTechOperation.Name = "cbTechOperation";
            this.cbTechOperation.Size = new System.Drawing.Size(206, 21);
            this.cbTechOperation.TabIndex = 6;
            this.cbTechOperation.SelectionChangeCommitted += new System.EventHandler(this.cbTechOperation_SelectionChangeCommitted);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Вид обработки";
            // 
            // cbMachine
            // 
            this.cbMachine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMachine.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.techProcessParamsBindingSource, "Machine", true));
            this.cbMachine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMachine.FormattingEnabled = true;
            this.cbMachine.Location = new System.Drawing.Point(94, 3);
            this.cbMachine.Name = "cbMachine";
            this.cbMachine.Size = new System.Drawing.Size(234, 21);
            this.cbMachine.TabIndex = 0;
            this.cbMachine.SelectionChangeCommitted += new System.EventHandler(this.cbMachine_SelectionChangeCommitted);
            // 
            // techProcessParamsBindingSource
            // 
            this.techProcessParamsBindingSource.DataSource = typeof(CAM.TechProcessParams);
            // 
            // cbMaterial
            // 
            this.cbMaterial.AllowDrop = true;
            this.cbMaterial.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMaterial.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.techProcessParamsBindingSource, "Material", true));
            this.cbMaterial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMaterial.FormattingEnabled = true;
            this.cbMaterial.Location = new System.Drawing.Point(91, 19);
            this.cbMaterial.Name = "cbMaterial";
            this.cbMaterial.Size = new System.Drawing.Size(225, 21);
            this.cbMaterial.TabIndex = 0;
            // 
            // lbMaterial
            // 
            this.lbMaterial.AutoSize = true;
            this.lbMaterial.Location = new System.Drawing.Point(7, 22);
            this.lbMaterial.Name = "lbMaterial";
            this.lbMaterial.Size = new System.Drawing.Size(57, 13);
            this.lbMaterial.TabIndex = 1;
            this.lbMaterial.Text = "Материал";
            // 
            // lbMachine
            // 
            this.lbMachine.AutoSize = true;
            this.lbMachine.Location = new System.Drawing.Point(6, 6);
            this.lbMachine.Name = "lbMachine";
            this.lbMachine.Size = new System.Drawing.Size(43, 13);
            this.lbMachine.TabIndex = 0;
            this.lbMachine.Text = "Станок";
            // 
            // gbTool
            // 
            this.gbTool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTool.Controls.Add(this.button1);
            this.gbTool.Controls.Add(this.edThickness);
            this.gbTool.Controls.Add(this.label2);
            this.gbTool.Controls.Add(this.edDiameter);
            this.gbTool.Controls.Add(this.label1);
            this.gbTool.Controls.Add(this.edToolNumber);
            this.gbTool.Controls.Add(this.lbToolNumber);
            this.gbTool.Location = new System.Drawing.Point(3, 136);
            this.gbTool.Name = "gbTool";
            this.gbTool.Size = new System.Drawing.Size(325, 89);
            this.gbTool.TabIndex = 2;
            this.gbTool.TabStop = false;
            this.gbTool.Text = "Инструмент";
            // 
            // edThickness
            // 
            this.edThickness.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edThickness.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.techProcessParamsBindingSource, "ToolThickness", true));
            this.edThickness.Location = new System.Drawing.Point(91, 63);
            this.edThickness.Multiline = true;
            this.edThickness.Name = "edThickness";
            this.edThickness.Size = new System.Drawing.Size(225, 20);
            this.edThickness.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Толщина";
            // 
            // edDiameter
            // 
            this.edDiameter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edDiameter.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.techProcessParamsBindingSource, "ToolDiameter", true));
            this.edDiameter.Location = new System.Drawing.Point(91, 41);
            this.edDiameter.Multiline = true;
            this.edDiameter.Name = "edDiameter";
            this.edDiameter.Size = new System.Drawing.Size(225, 20);
            this.edDiameter.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Диаметр";
            // 
            // edToolNumber
            // 
            this.edToolNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edToolNumber.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.techProcessParamsBindingSource, "ToolNumber", true));
            this.edToolNumber.Location = new System.Drawing.Point(91, 19);
            this.edToolNumber.Name = "edToolNumber";
            this.edToolNumber.Size = new System.Drawing.Size(190, 20);
            this.edToolNumber.TabIndex = 0;
            this.edToolNumber.Validating += new System.ComponentModel.CancelEventHandler(this.edToolNumber_Validating);
            // 
            // lbToolNumber
            // 
            this.lbToolNumber.AutoSize = true;
            this.lbToolNumber.Location = new System.Drawing.Point(7, 22);
            this.lbToolNumber.Name = "lbToolNumber";
            this.lbToolNumber.Size = new System.Drawing.Size(41, 13);
            this.lbToolNumber.TabIndex = 3;
            this.lbToolNumber.Text = "Номер";
            // 
            // gbBillet
            // 
            this.gbBillet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbBillet.Controls.Add(this.edBilletThickness);
            this.gbBillet.Controls.Add(this.label4);
            this.gbBillet.Controls.Add(this.cbMaterial);
            this.gbBillet.Controls.Add(this.lbMaterial);
            this.gbBillet.Location = new System.Drawing.Point(3, 57);
            this.gbBillet.Name = "gbBillet";
            this.gbBillet.Size = new System.Drawing.Size(325, 73);
            this.gbBillet.TabIndex = 1;
            this.gbBillet.TabStop = false;
            this.gbBillet.Text = "Заготовка";
            // 
            // edBilletThickness
            // 
            this.edBilletThickness.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edBilletThickness.AutoCompleteCustomSource.AddRange(new string[] {
            "20",
            "30"});
            this.edBilletThickness.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.edBilletThickness.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.edBilletThickness.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.techProcessParamsBindingSource, "BilletThickness", true));
            this.edBilletThickness.Location = new System.Drawing.Point(91, 42);
            this.edBilletThickness.Multiline = true;
            this.edBilletThickness.Name = "edBilletThickness";
            this.edBilletThickness.Size = new System.Drawing.Size(224, 20);
            this.edBilletThickness.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Толщина";
            // 
            // edFrequency
            // 
            this.edFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edFrequency.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.techProcessParamsBindingSource, "Frequency", true));
            this.edFrequency.Location = new System.Drawing.Point(94, 231);
            this.edFrequency.Multiline = true;
            this.edFrequency.Name = "edFrequency";
            this.edFrequency.Size = new System.Drawing.Size(225, 20);
            this.edFrequency.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 234);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Шпиндель";
            // 
            // edZSafety
            // 
            this.edZSafety.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edZSafety.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.techProcessParamsBindingSource, "ZSafety", true));
            this.edZSafety.Location = new System.Drawing.Point(94, 276);
            this.edZSafety.Multiline = true;
            this.edZSafety.Name = "edZSafety";
            this.edZSafety.Size = new System.Drawing.Size(225, 20);
            this.edZSafety.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 279);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Z безопасн.";
            // 
            // edPenetrationRate
            // 
            this.edPenetrationRate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edPenetrationRate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.techProcessParamsBindingSource, "PenetrationRate", true));
            this.edPenetrationRate.Location = new System.Drawing.Point(94, 253);
            this.edPenetrationRate.Multiline = true;
            this.edPenetrationRate.Name = "edPenetrationRate";
            this.edPenetrationRate.Size = new System.Drawing.Size(224, 20);
            this.edPenetrationRate.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 256);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Скорость мал.";
            // 
            // pParams
            // 
            this.pParams.Controls.Add(this.bTechOperationParams);
            this.pParams.Controls.Add(this.cbTechOperation);
            this.pParams.Controls.Add(this.label3);
            this.pParams.Controls.Add(this.cbMachine);
            this.pParams.Controls.Add(this.edPenetrationRate);
            this.pParams.Controls.Add(this.gbTool);
            this.pParams.Controls.Add(this.label7);
            this.pParams.Controls.Add(this.lbMachine);
            this.pParams.Controls.Add(this.edZSafety);
            this.pParams.Controls.Add(this.gbBillet);
            this.pParams.Controls.Add(this.label6);
            this.pParams.Controls.Add(this.label5);
            this.pParams.Controls.Add(this.edFrequency);
            this.pParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pParams.Location = new System.Drawing.Point(0, 0);
            this.pParams.Name = "pParams";
            this.pParams.Size = new System.Drawing.Size(331, 471);
            this.pParams.TabIndex = 15;
            // 
            // bTechOperationParams
            // 
            this.bTechOperationParams.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bTechOperationParams.Location = new System.Drawing.Point(306, 30);
            this.bTechOperationParams.Name = "bTechOperationParams";
            this.bTechOperationParams.Size = new System.Drawing.Size(21, 21);
            this.bTechOperationParams.TabIndex = 14;
            this.bTechOperationParams.Text = "…";
            this.toolTip1.SetToolTip(this.bTechOperationParams, "Параметры обработки");
            this.bTechOperationParams.UseVisualStyleBackColor = true;
            this.bTechOperationParams.Click += new System.EventHandler(this.bTechOperationParams_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(287, 18);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(21, 21);
            this.button1.TabIndex = 8;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TechProcessParamsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(0, 350);
            this.Controls.Add(this.pParams);
            this.Name = "TechProcessParamsView";
            this.Size = new System.Drawing.Size(331, 471);
            ((System.ComponentModel.ISupportInitialize)(this.techProcessParamsBindingSource)).EndInit();
            this.gbTool.ResumeLayout(false);
            this.gbTool.PerformLayout();
            this.gbBillet.ResumeLayout(false);
            this.gbBillet.PerformLayout();
            this.pParams.ResumeLayout(false);
            this.pParams.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbTool;
        private System.Windows.Forms.TextBox edThickness;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox edDiameter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox edToolNumber;
        private System.Windows.Forms.Label lbToolNumber;
        private System.Windows.Forms.ComboBox cbMaterial;
        private System.Windows.Forms.ComboBox cbMachine;
        private System.Windows.Forms.Label lbMaterial;
        private System.Windows.Forms.Label lbMachine;
        private System.Windows.Forms.ComboBox cbTechOperation;
        private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox gbBillet;
		private System.Windows.Forms.TextBox edBilletThickness;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox edFrequency;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox edZSafety;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox edPenetrationRate;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Panel pParams;
		private System.Windows.Forms.BindingSource techProcessParamsBindingSource;
        private System.Windows.Forms.Button bTechOperationParams;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button button1;
    }
}
