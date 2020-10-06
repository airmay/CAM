namespace CAM.TechProcesses.SectionProfile
{
    partial class SectionProfileTechProcessView
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
            this.tbZSafety = new System.Windows.Forms.TextBox();
            this.sectionProfileTechProcessBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.cbNormal = new System.Windows.Forms.CheckBox();
            this.tbPenetrationFeed = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cbMaterial = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbFrequency = new System.Windows.Forms.TextBox();
            this.lbFrequency = new System.Windows.Forms.Label();
            this.bObjects = new System.Windows.Forms.Button();
            this.tbObjects = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbTool = new System.Windows.Forms.TextBox();
            this.bTool = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tbStep = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDeparture = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbCuttingFeed = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.bRail = new System.Windows.Forms.Button();
            this.tbRail = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbLength = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cbMachine = new System.Windows.Forms.ComboBox();
            this.lbMachine = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.sectionProfileTechProcessBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tbZSafety
            // 
            this.tbZSafety.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbZSafety.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.sectionProfileTechProcessBindingSource, "ZSafety", true));
            this.tbZSafety.Location = new System.Drawing.Point(101, 336);
            this.tbZSafety.Name = "tbZSafety";
            this.tbZSafety.Size = new System.Drawing.Size(152, 20);
            this.tbZSafety.TabIndex = 120;
            // 
            // sectionProfileTechProcessBindingSource
            // 
            this.sectionProfileTechProcessBindingSource.DataSource = typeof(CAM.TechProcesses.SectionProfile.SectionProfileTechProcess);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 339);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 113;
            this.label5.Text = "Z безопасности";
            // 
            // cbNormal
            // 
            this.cbNormal.AutoSize = true;
            this.cbNormal.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.sectionProfileTechProcessBindingSource, "IsNormal", true));
            this.cbNormal.Location = new System.Drawing.Point(5, 243);
            this.cbNormal.Name = "cbNormal";
            this.cbNormal.Size = new System.Drawing.Size(87, 17);
            this.cbNormal.TabIndex = 80;
            this.cbNormal.Text = "По нормали";
            this.cbNormal.UseVisualStyleBackColor = true;
            this.cbNormal.Visible = false;
            // 
            // tbPenetrationFeed
            // 
            this.tbPenetrationFeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPenetrationFeed.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.sectionProfileTechProcessBindingSource, "PenetrationFeed", true));
            this.tbPenetrationFeed.Location = new System.Drawing.Point(101, 119);
            this.tbPenetrationFeed.Name = "tbPenetrationFeed";
            this.tbPenetrationFeed.Size = new System.Drawing.Size(152, 20);
            this.tbPenetrationFeed.TabIndex = 50;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(2, 122);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 13);
            this.label9.TabIndex = 110;
            this.label9.Text = "Скор. малая";
            // 
            // cbMaterial
            // 
            this.cbMaterial.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMaterial.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.sectionProfileTechProcessBindingSource, "Material", true));
            this.cbMaterial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMaterial.FormattingEnabled = true;
            this.cbMaterial.Location = new System.Drawing.Point(101, 27);
            this.cbMaterial.Name = "cbMaterial";
            this.cbMaterial.Size = new System.Drawing.Size(152, 21);
            this.cbMaterial.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 109;
            this.label1.Text = "Материал";
            // 
            // tbFrequency
            // 
            this.tbFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFrequency.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.sectionProfileTechProcessBindingSource, "Frequency", true));
            this.tbFrequency.Location = new System.Drawing.Point(101, 74);
            this.tbFrequency.Name = "tbFrequency";
            this.tbFrequency.Size = new System.Drawing.Size(152, 20);
            this.tbFrequency.TabIndex = 30;
            // 
            // lbFrequency
            // 
            this.lbFrequency.AutoSize = true;
            this.lbFrequency.Location = new System.Drawing.Point(2, 77);
            this.lbFrequency.Name = "lbFrequency";
            this.lbFrequency.Size = new System.Drawing.Size(58, 13);
            this.lbFrequency.TabIndex = 108;
            this.lbFrequency.Text = "Шпиндель";
            // 
            // bObjects
            // 
            this.bObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bObjects.Location = new System.Drawing.Point(233, 152);
            this.bObjects.Name = "bObjects";
            this.bObjects.Size = new System.Drawing.Size(21, 21);
            this.bObjects.TabIndex = 107;
            this.bObjects.TabStop = false;
            this.bObjects.Text = "۞";
            this.bObjects.UseVisualStyleBackColor = true;
            this.bObjects.Click += new System.EventHandler(this.bObjects_Click);
            // 
            // tbObjects
            // 
            this.tbObjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbObjects.Location = new System.Drawing.Point(101, 152);
            this.tbObjects.Name = "tbObjects";
            this.tbObjects.ReadOnly = true;
            this.tbObjects.Size = new System.Drawing.Size(130, 20);
            this.tbObjects.TabIndex = 60;
            this.tbObjects.Enter += new System.EventHandler(this.tbObjects_Enter);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(2, 156);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 106;
            this.label8.Text = "Профиль";
            // 
            // tbTool
            // 
            this.tbTool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTool.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tbTool.Location = new System.Drawing.Point(101, 51);
            this.tbTool.Name = "tbTool";
            this.tbTool.ReadOnly = true;
            this.tbTool.Size = new System.Drawing.Size(130, 20);
            this.tbTool.TabIndex = 20;
            // 
            // bTool
            // 
            this.bTool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bTool.Location = new System.Drawing.Point(233, 51);
            this.bTool.Name = "bTool";
            this.bTool.Size = new System.Drawing.Size(20, 20);
            this.bTool.TabIndex = 105;
            this.bTool.TabStop = false;
            this.bTool.Text = "Ξ";
            this.bTool.UseVisualStyleBackColor = true;
            this.bTool.Click += new System.EventHandler(this.bTool_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(2, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 104;
            this.label6.Text = "Инструмент";
            // 
            // tbStep
            // 
            this.tbStep.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbStep.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.sectionProfileTechProcessBindingSource, "Step", true));
            this.tbStep.Location = new System.Drawing.Point(101, 267);
            this.tbStep.Name = "tbStep";
            this.tbStep.Size = new System.Drawing.Size(152, 20);
            this.tbStep.TabIndex = 90;
            this.tbStep.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 270);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 125;
            this.label2.Text = "Шаг";
            this.label2.Visible = false;
            // 
            // tbDeparture
            // 
            this.tbDeparture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDeparture.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.sectionProfileTechProcessBindingSource, "Departure", true));
            this.tbDeparture.Location = new System.Drawing.Point(101, 290);
            this.tbDeparture.Name = "tbDeparture";
            this.tbDeparture.Size = new System.Drawing.Size(152, 20);
            this.tbDeparture.TabIndex = 100;
            this.tbDeparture.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 293);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 13);
            this.label7.TabIndex = 124;
            this.label7.Text = "Выезд";
            this.label7.Visible = false;
            // 
            // textBox5
            // 
            this.textBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox5.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.sectionProfileTechProcessBindingSource, "Delta", true));
            this.textBox5.Location = new System.Drawing.Point(101, 313);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(152, 20);
            this.textBox5.TabIndex = 110;
            this.textBox5.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 316);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 123;
            this.label3.Text = "Припуск";
            this.label3.Visible = false;
            // 
            // tbCuttingFeed
            // 
            this.tbCuttingFeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCuttingFeed.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.sectionProfileTechProcessBindingSource, "CuttingFeed", true));
            this.tbCuttingFeed.Location = new System.Drawing.Point(101, 96);
            this.tbCuttingFeed.Name = "tbCuttingFeed";
            this.tbCuttingFeed.Size = new System.Drawing.Size(152, 20);
            this.tbCuttingFeed.TabIndex = 40;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 122;
            this.label4.Text = "Подача";
            // 
            // bRail
            // 
            this.bRail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bRail.Location = new System.Drawing.Point(233, 189);
            this.bRail.Name = "bRail";
            this.bRail.Size = new System.Drawing.Size(21, 21);
            this.bRail.TabIndex = 128;
            this.bRail.TabStop = false;
            this.bRail.Text = "۞";
            this.bRail.UseVisualStyleBackColor = true;
            this.bRail.Click += new System.EventHandler(this.bRail_Click);
            // 
            // tbRail
            // 
            this.tbRail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRail.Location = new System.Drawing.Point(101, 189);
            this.tbRail.Name = "tbRail";
            this.tbRail.ReadOnly = true;
            this.tbRail.Size = new System.Drawing.Size(130, 20);
            this.tbRail.TabIndex = 70;
            this.tbRail.Enter += new System.EventHandler(this.tbRail_Enter);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(2, 193);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(86, 13);
            this.label10.TabIndex = 127;
            this.label10.Text = "Направляющая";
            // 
            // tbLength
            // 
            this.tbLength.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLength.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.sectionProfileTechProcessBindingSource, "Length", true));
            this.tbLength.Location = new System.Drawing.Point(101, 213);
            this.tbLength.Name = "tbLength";
            this.tbLength.Size = new System.Drawing.Size(152, 20);
            this.tbLength.TabIndex = 75;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(2, 216);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(88, 13);
            this.label11.TabIndex = 130;
            this.label11.Text = "Длина направл.";
            // 
            // cbMachine
            // 
            this.cbMachine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMachine.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.sectionProfileTechProcessBindingSource, "MachineType", true));
            this.cbMachine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMachine.FormattingEnabled = true;
            this.cbMachine.Location = new System.Drawing.Point(101, 3);
            this.cbMachine.Name = "cbMachine";
            this.cbMachine.Size = new System.Drawing.Size(153, 21);
            this.cbMachine.TabIndex = 5;
            // 
            // lbMachine
            // 
            this.lbMachine.AutoSize = true;
            this.lbMachine.Location = new System.Drawing.Point(2, 6);
            this.lbMachine.Name = "lbMachine";
            this.lbMachine.Size = new System.Drawing.Size(43, 13);
            this.lbMachine.TabIndex = 132;
            this.lbMachine.Text = "Станок";
            // 
            // SectionProfileTechProcessView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbMachine);
            this.Controls.Add(this.lbMachine);
            this.Controls.Add(this.tbLength);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.bRail);
            this.Controls.Add(this.tbRail);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tbStep);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbDeparture);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbCuttingFeed);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbZSafety);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbNormal);
            this.Controls.Add(this.tbPenetrationFeed);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cbMaterial);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbFrequency);
            this.Controls.Add(this.lbFrequency);
            this.Controls.Add(this.bObjects);
            this.Controls.Add(this.tbObjects);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tbTool);
            this.Controls.Add(this.bTool);
            this.Controls.Add(this.label6);
            this.Name = "SectionProfileTechProcessView";
            this.Size = new System.Drawing.Size(257, 540);
            ((System.ComponentModel.ISupportInitialize)(this.sectionProfileTechProcessBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbZSafety;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbNormal;
        private System.Windows.Forms.TextBox tbPenetrationFeed;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbMaterial;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbFrequency;
        private System.Windows.Forms.Label lbFrequency;
        private System.Windows.Forms.Button bObjects;
        private System.Windows.Forms.TextBox tbObjects;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbTool;
        private System.Windows.Forms.Button bTool;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbStep;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDeparture;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbCuttingFeed;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bRail;
        private System.Windows.Forms.TextBox tbRail;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.BindingSource sectionProfileTechProcessBindingSource;
        private System.Windows.Forms.TextBox tbLength;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cbMachine;
        private System.Windows.Forms.Label lbMachine;
    }
}
