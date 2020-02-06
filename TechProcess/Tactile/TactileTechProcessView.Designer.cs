namespace CAM.Tactile
{
    partial class TactileTechProcessView
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
            this.cbMachine = new System.Windows.Forms.ComboBox();
            this.lbMachine = new System.Windows.Forms.Label();
            this.tbTransitionFeed = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbDeparture = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbDepth = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbBandWidth = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbBandStart = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbBandSpacing = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.bTool = new System.Windows.Forms.Button();
            this.tbTool = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tbProcessingArea = new System.Windows.Forms.TextBox();
            this.tbOrigin = new System.Windows.Forms.TextBox();
            this.bProcessingArea = new System.Windows.Forms.Button();
            this.bOrigin = new System.Windows.Forms.Button();
            this.tbPenetrationFeed = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbFrequency = new System.Windows.Forms.TextBox();
            this.lbFrequency = new System.Windows.Forms.Label();
            this.tactileTechProcessBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tactileTechProcessParamsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tactileTechProcessBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tactileTechProcessParamsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // cbMachine
            // 
            this.cbMachine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMachine.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessBindingSource, "MachineType", true));
            this.cbMachine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMachine.FormattingEnabled = true;
            this.cbMachine.Location = new System.Drawing.Point(102, 3);
            this.cbMachine.Name = "cbMachine";
            this.cbMachine.Size = new System.Drawing.Size(152, 21);
            this.cbMachine.TabIndex = 1;
            // 
            // lbMachine
            // 
            this.lbMachine.AutoSize = true;
            this.lbMachine.Location = new System.Drawing.Point(3, 6);
            this.lbMachine.Name = "lbMachine";
            this.lbMachine.Size = new System.Drawing.Size(43, 13);
            this.lbMachine.TabIndex = 2;
            this.lbMachine.Text = "Станок";
            // 
            // tbTransitionFeed
            // 
            this.tbTransitionFeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTransitionFeed.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessParamsBindingSource, "TransitionFeed", true));
            this.tbTransitionFeed.Location = new System.Drawing.Point(102, 111);
            this.tbTransitionFeed.Name = "tbTransitionFeed";
            this.tbTransitionFeed.Size = new System.Drawing.Size(152, 20);
            this.tbTransitionFeed.TabIndex = 4;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 114);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(50, 13);
            this.label11.TabIndex = 31;
            this.label11.Text = "Переход";
            // 
            // tbDeparture
            // 
            this.tbDeparture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDeparture.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessParamsBindingSource, "Departure", true));
            this.tbDeparture.Location = new System.Drawing.Point(102, 88);
            this.tbDeparture.Name = "tbDeparture";
            this.tbDeparture.Size = new System.Drawing.Size(152, 20);
            this.tbDeparture.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 29;
            this.label5.Text = "Выезд";
            // 
            // tbDepth
            // 
            this.tbDepth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDepth.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessParamsBindingSource, "Depth", true));
            this.tbDepth.Location = new System.Drawing.Point(102, 302);
            this.tbDepth.Name = "tbDepth";
            this.tbDepth.Size = new System.Drawing.Size(152, 20);
            this.tbDepth.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 305);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 27;
            this.label4.Text = "Глубина";
            // 
            // tbBandWidth
            // 
            this.tbBandWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBandWidth.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessParamsBindingSource, "BandWidth", true));
            this.tbBandWidth.Location = new System.Drawing.Point(102, 233);
            this.tbBandWidth.Name = "tbBandWidth";
            this.tbBandWidth.Size = new System.Drawing.Size(152, 20);
            this.tbBandWidth.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 236);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Ширина полосы";
            // 
            // tbBandStart
            // 
            this.tbBandStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBandStart.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessParamsBindingSource, "BandStart", true));
            this.tbBandStart.Location = new System.Drawing.Point(102, 279);
            this.tbBandStart.Name = "tbBandStart";
            this.tbBandStart.Size = new System.Drawing.Size(152, 20);
            this.tbBandStart.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 282);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Начало полосы";
            // 
            // tbBandSpacing
            // 
            this.tbBandSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBandSpacing.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessParamsBindingSource, "BandSpacing", true));
            this.tbBandSpacing.Location = new System.Drawing.Point(102, 256);
            this.tbBandSpacing.Name = "tbBandSpacing";
            this.tbBandSpacing.Size = new System.Drawing.Size(152, 20);
            this.tbBandSpacing.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 259);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Расст.между пол.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 33;
            this.label6.Text = "Инструмент";
            // 
            // bTool
            // 
            this.bTool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bTool.Location = new System.Drawing.Point(234, 27);
            this.bTool.Name = "bTool";
            this.bTool.Size = new System.Drawing.Size(20, 20);
            this.bTool.TabIndex = 35;
            this.bTool.TabStop = false;
            this.bTool.Text = "Ξ";
            this.bTool.UseVisualStyleBackColor = true;
            this.bTool.Click += new System.EventHandler(this.bTool_Click);
            // 
            // tbTool
            // 
            this.tbTool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTool.Enabled = false;
            this.tbTool.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tbTool.Location = new System.Drawing.Point(102, 27);
            this.tbTool.Name = "tbTool";
            this.tbTool.Size = new System.Drawing.Size(130, 20);
            this.tbTool.TabIndex = 36;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 175);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 13);
            this.label7.TabIndex = 37;
            this.label7.Text = "Объекты чертежа";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 199);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 13);
            this.label8.TabIndex = 38;
            this.label8.Text = "Начало координат";
            // 
            // tbProcessingArea
            // 
            this.tbProcessingArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProcessingArea.Enabled = false;
            this.tbProcessingArea.Location = new System.Drawing.Point(102, 172);
            this.tbProcessingArea.Name = "tbProcessingArea";
            this.tbProcessingArea.Size = new System.Drawing.Size(130, 20);
            this.tbProcessingArea.TabIndex = 39;
            // 
            // tbOrigin
            // 
            this.tbOrigin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOrigin.Enabled = false;
            this.tbOrigin.Location = new System.Drawing.Point(102, 195);
            this.tbOrigin.Name = "tbOrigin";
            this.tbOrigin.Size = new System.Drawing.Size(130, 20);
            this.tbOrigin.TabIndex = 40;
            // 
            // bProcessingArea
            // 
            this.bProcessingArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bProcessingArea.Location = new System.Drawing.Point(234, 171);
            this.bProcessingArea.Name = "bProcessingArea";
            this.bProcessingArea.Size = new System.Drawing.Size(21, 21);
            this.bProcessingArea.TabIndex = 41;
            this.bProcessingArea.TabStop = false;
            this.bProcessingArea.Text = "۞";
            this.bProcessingArea.UseVisualStyleBackColor = true;
            this.bProcessingArea.Click += new System.EventHandler(this.bProcessingArea_Click);
            // 
            // bOrigin
            // 
            this.bOrigin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bOrigin.Location = new System.Drawing.Point(234, 195);
            this.bOrigin.Name = "bOrigin";
            this.bOrigin.Size = new System.Drawing.Size(21, 21);
            this.bOrigin.TabIndex = 42;
            this.bOrigin.TabStop = false;
            this.bOrigin.Text = "۞";
            this.bOrigin.UseVisualStyleBackColor = true;
            this.bOrigin.Click += new System.EventHandler(this.bOrigin_Click);
            // 
            // tbPenetrationFeed
            // 
            this.tbPenetrationFeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPenetrationFeed.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessParamsBindingSource, "PenetrationFeed", true));
            this.tbPenetrationFeed.Location = new System.Drawing.Point(102, 134);
            this.tbPenetrationFeed.Name = "tbPenetrationFeed";
            this.tbPenetrationFeed.Size = new System.Drawing.Size(152, 20);
            this.tbPenetrationFeed.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 137);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 13);
            this.label9.TabIndex = 43;
            this.label9.Text = "Скор. малая";
            // 
            // tbFrequency
            // 
            this.tbFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFrequency.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessBindingSource, "Frequency", true));
            this.tbFrequency.Location = new System.Drawing.Point(102, 50);
            this.tbFrequency.Name = "tbFrequency";
            this.tbFrequency.Size = new System.Drawing.Size(152, 20);
            this.tbFrequency.TabIndex = 2;
            // 
            // lbFrequency
            // 
            this.lbFrequency.AutoSize = true;
            this.lbFrequency.Location = new System.Drawing.Point(3, 53);
            this.lbFrequency.Name = "lbFrequency";
            this.lbFrequency.Size = new System.Drawing.Size(58, 13);
            this.lbFrequency.TabIndex = 45;
            this.lbFrequency.Text = "Шпиндель";
            // 
            // tactileTechProcessBindingSource
            // 
            this.tactileTechProcessBindingSource.DataSource = typeof(CAM.Tactile.TactileTechProcess);
            // 
            // tactileTechProcessParamsBindingSource
            // 
            this.tactileTechProcessParamsBindingSource.DataSource = typeof(CAM.Tactile.TactileTechProcessParams);
            // 
            // TactileTechProcessView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbFrequency);
            this.Controls.Add(this.lbFrequency);
            this.Controls.Add(this.tbPenetrationFeed);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.bOrigin);
            this.Controls.Add(this.bProcessingArea);
            this.Controls.Add(this.tbOrigin);
            this.Controls.Add(this.tbProcessingArea);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbTool);
            this.Controls.Add(this.bTool);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbTransitionFeed);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.tbDeparture);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbDepth);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbBandWidth);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbBandStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbBandSpacing);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbMachine);
            this.Controls.Add(this.lbMachine);
            this.Name = "TactileTechProcessView";
            this.Size = new System.Drawing.Size(257, 540);
            ((System.ComponentModel.ISupportInitialize)(this.tactileTechProcessBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tactileTechProcessParamsBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbMachine;
        private System.Windows.Forms.Label lbMachine;
        private System.Windows.Forms.TextBox tbTransitionFeed;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbDeparture;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbDepth;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbBandWidth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbBandStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbBandSpacing;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button bTool;
        private System.Windows.Forms.TextBox tbTool;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbProcessingArea;
        private System.Windows.Forms.TextBox tbOrigin;
        private System.Windows.Forms.Button bProcessingArea;
        private System.Windows.Forms.Button bOrigin;
        private System.Windows.Forms.TextBox tbPenetrationFeed;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.BindingSource tactileTechProcessParamsBindingSource;
        private System.Windows.Forms.TextBox tbFrequency;
        private System.Windows.Forms.Label lbFrequency;
        private System.Windows.Forms.BindingSource tactileTechProcessBindingSource;
    }
}
