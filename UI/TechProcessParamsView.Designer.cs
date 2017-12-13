namespace CAM.UI
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
            this.gbTechProcessParams = new System.Windows.Forms.GroupBox();
            this.lbMachine = new System.Windows.Forms.Label();
            this.lbMaterial = new System.Windows.Forms.Label();
            this.cbMachine = new System.Windows.Forms.ComboBox();
            this.cbMaterial = new System.Windows.Forms.ComboBox();
            this.gbTool = new System.Windows.Forms.GroupBox();
            this.lbToolNumber = new System.Windows.Forms.Label();
            this.edToolNumber = new System.Windows.Forms.TextBox();
            this.edDiameter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.edThickness = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbTechOperation = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.gbTechOperation = new System.Windows.Forms.GroupBox();
            this.gbTechProcessParams.SuspendLayout();
            this.gbTool.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbTechProcessParams
            // 
            this.gbTechProcessParams.Controls.Add(this.cbTechOperation);
            this.gbTechProcessParams.Controls.Add(this.label3);
            this.gbTechProcessParams.Controls.Add(this.cbMachine);
            this.gbTechProcessParams.Controls.Add(this.cbMaterial);
            this.gbTechProcessParams.Controls.Add(this.lbMaterial);
            this.gbTechProcessParams.Controls.Add(this.lbMachine);
            this.gbTechProcessParams.Controls.Add(this.gbTool);
            this.gbTechProcessParams.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbTechProcessParams.Location = new System.Drawing.Point(0, 0);
            this.gbTechProcessParams.Name = "gbTechProcessParams";
            this.gbTechProcessParams.Size = new System.Drawing.Size(203, 211);
            this.gbTechProcessParams.TabIndex = 0;
            this.gbTechProcessParams.TabStop = false;
            this.gbTechProcessParams.Text = "Параметры тех. процесса";
            // 
            // lbMachine
            // 
            this.lbMachine.AutoSize = true;
            this.lbMachine.Location = new System.Drawing.Point(10, 22);
            this.lbMachine.Name = "lbMachine";
            this.lbMachine.Size = new System.Drawing.Size(43, 13);
            this.lbMachine.TabIndex = 0;
            this.lbMachine.Text = "Станок";
            // 
            // lbMaterial
            // 
            this.lbMaterial.AutoSize = true;
            this.lbMaterial.Location = new System.Drawing.Point(10, 49);
            this.lbMaterial.Name = "lbMaterial";
            this.lbMaterial.Size = new System.Drawing.Size(46, 13);
            this.lbMaterial.TabIndex = 1;
            this.lbMaterial.Text = "Камень";
            // 
            // cbMachine
            // 
            this.cbMachine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMachine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMachine.FormattingEnabled = true;
            this.cbMachine.Items.AddRange(new object[] {
            "ScemaLogic",
            "Krea"});
            this.cbMachine.Location = new System.Drawing.Point(69, 19);
            this.cbMachine.Name = "cbMachine";
            this.cbMachine.Size = new System.Drawing.Size(122, 21);
            this.cbMachine.TabIndex = 2;
            // 
            // cbMaterial
            // 
            this.cbMaterial.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMaterial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMaterial.FormattingEnabled = true;
            this.cbMaterial.Items.AddRange(new object[] {
            "Мрамор",
            "Гранит"});
            this.cbMaterial.Location = new System.Drawing.Point(69, 46);
            this.cbMaterial.Name = "cbMaterial";
            this.cbMaterial.Size = new System.Drawing.Size(122, 21);
            this.cbMaterial.TabIndex = 3;
            // 
            // gbTool
            // 
            this.gbTool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTool.Controls.Add(this.edThickness);
            this.gbTool.Controls.Add(this.label2);
            this.gbTool.Controls.Add(this.edDiameter);
            this.gbTool.Controls.Add(this.label1);
            this.gbTool.Controls.Add(this.edToolNumber);
            this.gbTool.Controls.Add(this.lbToolNumber);
            this.gbTool.Location = new System.Drawing.Point(3, 73);
            this.gbTool.Name = "gbTool";
            this.gbTool.Size = new System.Drawing.Size(188, 104);
            this.gbTool.TabIndex = 4;
            this.gbTool.TabStop = false;
            this.gbTool.Text = "Инструмент";
            // 
            // lbToolNumber
            // 
            this.lbToolNumber.AutoSize = true;
            this.lbToolNumber.Location = new System.Drawing.Point(6, 25);
            this.lbToolNumber.Name = "lbToolNumber";
            this.lbToolNumber.Size = new System.Drawing.Size(41, 13);
            this.lbToolNumber.TabIndex = 3;
            this.lbToolNumber.Text = "Номер";
            // 
            // edToolNumber
            // 
            this.edToolNumber.Location = new System.Drawing.Point(65, 22);
            this.edToolNumber.Multiline = true;
            this.edToolNumber.Name = "edToolNumber";
            this.edToolNumber.Size = new System.Drawing.Size(58, 20);
            this.edToolNumber.TabIndex = 4;
            // 
            // edDiameter
            // 
            this.edDiameter.Location = new System.Drawing.Point(65, 48);
            this.edDiameter.Multiline = true;
            this.edDiameter.Name = "edDiameter";
            this.edDiameter.Size = new System.Drawing.Size(58, 20);
            this.edDiameter.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Диаметр";
            // 
            // edThickness
            // 
            this.edThickness.Location = new System.Drawing.Point(65, 74);
            this.edThickness.Multiline = true;
            this.edThickness.Name = "edThickness";
            this.edThickness.Size = new System.Drawing.Size(58, 20);
            this.edThickness.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Толщина";
            // 
            // cbTechOperation
            // 
            this.cbTechOperation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTechOperation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTechOperation.FormattingEnabled = true;
            this.cbTechOperation.Items.AddRange(new object[] {
            "Распил",
            "Профилирование"});
            this.cbTechOperation.Location = new System.Drawing.Point(91, 183);
            this.cbTechOperation.Name = "cbTechOperation";
            this.cbTechOperation.Size = new System.Drawing.Size(100, 21);
            this.cbTechOperation.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 186);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Тех.операция";
            // 
            // gbTechOperation
            // 
            this.gbTechOperation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbTechOperation.Location = new System.Drawing.Point(0, 211);
            this.gbTechOperation.Name = "gbTechOperation";
            this.gbTechOperation.Size = new System.Drawing.Size(203, 216);
            this.gbTechOperation.TabIndex = 7;
            this.gbTechOperation.TabStop = false;
            this.gbTechOperation.Text = "Параметры тех. операции";
            // 
            // TechProcessParamsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbTechOperation);
            this.Controls.Add(this.gbTechProcessParams);
            this.Name = "TechProcessParamsView";
            this.Size = new System.Drawing.Size(203, 427);
            this.gbTechProcessParams.ResumeLayout(false);
            this.gbTechProcessParams.PerformLayout();
            this.gbTool.ResumeLayout(false);
            this.gbTool.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbTechProcessParams;
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
        private System.Windows.Forms.GroupBox gbTechOperation;
    }
}
