namespace CAM.TechProcesses.Polishing
{
    partial class PolishingTechProcessView
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
            this.bOrigin = new System.Windows.Forms.Button();
            this.tbOrigin = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbZSafety = new System.Windows.Forms.TextBox();
            this.polishingTechProcessBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.tbFeed = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.bObjects = new System.Windows.Forms.Button();
            this.tbObjects = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbFrequency = new System.Windows.Forms.TextBox();
            this.lbFrequency = new System.Windows.Forms.Label();
            this.cbMachine = new System.Windows.Forms.ComboBox();
            this.lbMachine = new System.Windows.Forms.Label();
            this.tbAngle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbStep1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbAmplitude = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbStep2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.polishingTechProcessBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // bOrigin
            // 
            this.bOrigin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bOrigin.Location = new System.Drawing.Point(234, 115);
            this.bOrigin.Name = "bOrigin";
            this.bOrigin.Size = new System.Drawing.Size(21, 21);
            this.bOrigin.TabIndex = 101;
            this.bOrigin.TabStop = false;
            this.bOrigin.Text = "۞";
            this.bOrigin.UseVisualStyleBackColor = true;
            this.bOrigin.Click += new System.EventHandler(this.bOrigin_Click);
            // 
            // tbOrigin
            // 
            this.tbOrigin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOrigin.Location = new System.Drawing.Point(102, 115);
            this.tbOrigin.Name = "tbOrigin";
            this.tbOrigin.ReadOnly = true;
            this.tbOrigin.Size = new System.Drawing.Size(130, 20);
            this.tbOrigin.TabIndex = 50;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 119);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 13);
            this.label8.TabIndex = 100;
            this.label8.Text = "Начало координат";
            // 
            // tbZSafety
            // 
            this.tbZSafety.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbZSafety.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.polishingTechProcessBindingSource, "ZSafety", true));
            this.tbZSafety.Location = new System.Drawing.Point(102, 81);
            this.tbZSafety.Name = "tbZSafety";
            this.tbZSafety.Size = new System.Drawing.Size(152, 20);
            this.tbZSafety.TabIndex = 40;
            // 
            // polishingTechProcessBindingSource
            // 
            this.polishingTechProcessBindingSource.DataSource = typeof(CAM.TechProcesses.Polishing.PolishingTechProcess);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 97;
            this.label5.Text = "Z безопасности";
            // 
            // tbFeed
            // 
            this.tbFeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFeed.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.polishingTechProcessBindingSource, "Feed", true));
            this.tbFeed.Location = new System.Drawing.Point(102, 58);
            this.tbFeed.Name = "tbFeed";
            this.tbFeed.Size = new System.Drawing.Size(152, 20);
            this.tbFeed.TabIndex = 30;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 94;
            this.label6.Text = "Подача";
            // 
            // bObjects
            // 
            this.bObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bObjects.Location = new System.Drawing.Point(233, 138);
            this.bObjects.Name = "bObjects";
            this.bObjects.Size = new System.Drawing.Size(21, 21);
            this.bObjects.TabIndex = 92;
            this.bObjects.TabStop = false;
            this.bObjects.Text = "۞";
            this.bObjects.UseVisualStyleBackColor = true;
            this.bObjects.Click += new System.EventHandler(this.bObjects_Click);
            // 
            // tbObjects
            // 
            this.tbObjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbObjects.Location = new System.Drawing.Point(102, 139);
            this.tbObjects.Name = "tbObjects";
            this.tbObjects.ReadOnly = true;
            this.tbObjects.Size = new System.Drawing.Size(130, 20);
            this.tbObjects.TabIndex = 60;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(2, 142);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(42, 13);
            this.label10.TabIndex = 91;
            this.label10.Text = "Контур";
            // 
            // tbFrequency
            // 
            this.tbFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFrequency.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.polishingTechProcessBindingSource, "Frequency", true));
            this.tbFrequency.Location = new System.Drawing.Point(102, 26);
            this.tbFrequency.Name = "tbFrequency";
            this.tbFrequency.Size = new System.Drawing.Size(152, 20);
            this.tbFrequency.TabIndex = 20;
            // 
            // lbFrequency
            // 
            this.lbFrequency.AutoSize = true;
            this.lbFrequency.Location = new System.Drawing.Point(3, 29);
            this.lbFrequency.Name = "lbFrequency";
            this.lbFrequency.Size = new System.Drawing.Size(58, 13);
            this.lbFrequency.TabIndex = 90;
            this.lbFrequency.Text = "Шпиндель";
            // 
            // cbMachine
            // 
            this.cbMachine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMachine.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.polishingTechProcessBindingSource, "MachineType", true));
            this.cbMachine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMachine.FormattingEnabled = true;
            this.cbMachine.Location = new System.Drawing.Point(102, 2);
            this.cbMachine.Name = "cbMachine";
            this.cbMachine.Size = new System.Drawing.Size(152, 21);
            this.cbMachine.TabIndex = 10;
            // 
            // lbMachine
            // 
            this.lbMachine.AutoSize = true;
            this.lbMachine.Location = new System.Drawing.Point(3, 5);
            this.lbMachine.Name = "lbMachine";
            this.lbMachine.Size = new System.Drawing.Size(43, 13);
            this.lbMachine.TabIndex = 103;
            this.lbMachine.Text = "Станок";
            // 
            // tbAngle
            // 
            this.tbAngle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAngle.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.polishingTechProcessBindingSource, "Angle", true));
            this.tbAngle.Location = new System.Drawing.Point(102, 175);
            this.tbAngle.Name = "tbAngle";
            this.tbAngle.Size = new System.Drawing.Size(152, 20);
            this.tbAngle.TabIndex = 70;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 178);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 105;
            this.label1.Text = "Угол";
            // 
            // tbStep1
            // 
            this.tbStep1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbStep1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.polishingTechProcessBindingSource, "Step1", true));
            this.tbStep1.Location = new System.Drawing.Point(102, 198);
            this.tbStep1.Name = "tbStep1";
            this.tbStep1.Size = new System.Drawing.Size(152, 20);
            this.tbStep1.TabIndex = 80;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 201);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 107;
            this.label2.Text = "Шаг1";
            // 
            // tbAmplitude
            // 
            this.tbAmplitude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAmplitude.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.polishingTechProcessBindingSource, "Amplitude", true));
            this.tbAmplitude.Location = new System.Drawing.Point(102, 221);
            this.tbAmplitude.Name = "tbAmplitude";
            this.tbAmplitude.Size = new System.Drawing.Size(152, 20);
            this.tbAmplitude.TabIndex = 90;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 224);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 109;
            this.label3.Text = "Амплитуда";
            // 
            // tbStep2
            // 
            this.tbStep2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbStep2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.polishingTechProcessBindingSource, "Step2", true));
            this.tbStep2.Location = new System.Drawing.Point(102, 244);
            this.tbStep2.Name = "tbStep2";
            this.tbStep2.Size = new System.Drawing.Size(152, 20);
            this.tbStep2.TabIndex = 100;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 247);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 111;
            this.label4.Text = "Шаг2";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.polishingTechProcessBindingSource, "IsRandomStepCount", true));
            this.checkBox1.Location = new System.Drawing.Point(6, 273);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(165, 17);
            this.checkBox1.TabIndex = 112;
            this.checkBox1.Text = "Изменять количество волн";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.polishingTechProcessBindingSource, "IsRandomStepParams", true));
            this.checkBox2.Location = new System.Drawing.Point(6, 296);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(164, 17);
            this.checkBox2.TabIndex = 113;
            this.checkBox2.Text = "Изменять параметры волн";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // PolishingTechProcessView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.tbStep2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbAmplitude);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbStep1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbAngle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbMachine);
            this.Controls.Add(this.lbMachine);
            this.Controls.Add(this.bOrigin);
            this.Controls.Add(this.tbOrigin);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tbZSafety);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbFeed);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.bObjects);
            this.Controls.Add(this.tbObjects);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tbFrequency);
            this.Controls.Add(this.lbFrequency);
            this.Name = "PolishingTechProcessView";
            this.Size = new System.Drawing.Size(257, 540);
            ((System.ComponentModel.ISupportInitialize)(this.polishingTechProcessBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bOrigin;
        private System.Windows.Forms.TextBox tbOrigin;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbZSafety;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbFeed;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button bObjects;
        private System.Windows.Forms.TextBox tbObjects;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbFrequency;
        private System.Windows.Forms.Label lbFrequency;
        private System.Windows.Forms.ComboBox cbMachine;
        private System.Windows.Forms.Label lbMachine;
        private System.Windows.Forms.TextBox tbAngle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbStep1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbAmplitude;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbStep2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.BindingSource polishingTechProcessBindingSource;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
    }
}
