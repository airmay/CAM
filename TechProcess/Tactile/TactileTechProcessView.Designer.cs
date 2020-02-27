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
            this.tbBandStart1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbBandSpacing = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.bTool = new System.Windows.Forms.Button();
            this.tbTool = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tbContour = new System.Windows.Forms.TextBox();
            this.tbOrigin = new System.Windows.Forms.TextBox();
            this.bContour = new System.Windows.Forms.Button();
            this.bOrigin = new System.Windows.Forms.Button();
            this.tbPenetrationFeed = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbFrequency = new System.Windows.Forms.TextBox();
            this.lbFrequency = new System.Windows.Forms.Label();
            this.bObjects = new System.Windows.Forms.Button();
            this.tbObjects = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbType = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbBandStart2 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
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
            this.cbMachine.Enabled = false;
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
            this.tbTransitionFeed.Location = new System.Drawing.Point(102, 134);
            this.tbTransitionFeed.Name = "tbTransitionFeed";
            this.tbTransitionFeed.Size = new System.Drawing.Size(152, 20);
            this.tbTransitionFeed.TabIndex = 11;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 137);
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
            this.tbDeparture.Location = new System.Drawing.Point(102, 111);
            this.tbDeparture.Name = "tbDeparture";
            this.tbDeparture.Size = new System.Drawing.Size(152, 20);
            this.tbDeparture.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 114);
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
            this.tbDepth.Location = new System.Drawing.Point(102, 88);
            this.tbDepth.Name = "tbDepth";
            this.tbDepth.Size = new System.Drawing.Size(152, 20);
            this.tbDepth.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 27;
            this.label4.Text = "Глубина";
            // 
            // tbBandWidth
            // 
            this.tbBandWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBandWidth.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessBindingSource, "BandWidth", true));
            this.tbBandWidth.Location = new System.Drawing.Point(102, 302);
            this.tbBandWidth.Name = "tbBandWidth";
            this.tbBandWidth.Size = new System.Drawing.Size(152, 20);
            this.tbBandWidth.TabIndex = 42;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 305);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Ширина полосы";
            // 
            // tbBandStart1
            // 
            this.tbBandStart1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBandStart1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessBindingSource, "BandStart1", true));
            this.tbBandStart1.Location = new System.Drawing.Point(102, 348);
            this.tbBandStart1.Name = "tbBandStart1";
            this.tbBandStart1.Size = new System.Drawing.Size(152, 20);
            this.tbBandStart1.TabIndex = 44;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 351);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Начало полосы 1";
            // 
            // tbBandSpacing
            // 
            this.tbBandSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBandSpacing.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessBindingSource, "BandSpacing", true));
            this.tbBandSpacing.Location = new System.Drawing.Point(102, 325);
            this.tbBandSpacing.Name = "tbBandSpacing";
            this.tbBandSpacing.Size = new System.Drawing.Size(152, 20);
            this.tbBandSpacing.TabIndex = 43;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 328);
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
            this.toolTip1.SetToolTip(this.bTool, "Выбор инструмента");
            this.bTool.UseVisualStyleBackColor = true;
            this.bTool.Click += new System.EventHandler(this.bTool_Click);
            // 
            // tbTool
            // 
            this.tbTool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTool.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tbTool.Location = new System.Drawing.Point(102, 27);
            this.tbTool.Name = "tbTool";
            this.tbTool.ReadOnly = true;
            this.tbTool.Size = new System.Drawing.Size(130, 20);
            this.tbTool.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 221);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 13);
            this.label7.TabIndex = 37;
            this.label7.Text = "Контур плитки";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 184);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 13);
            this.label8.TabIndex = 38;
            this.label8.Text = "Начало координат";
            // 
            // tbContour
            // 
            this.tbContour.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbContour.Location = new System.Drawing.Point(102, 218);
            this.tbContour.Name = "tbContour";
            this.tbContour.ReadOnly = true;
            this.tbContour.Size = new System.Drawing.Size(130, 20);
            this.tbContour.TabIndex = 21;
            this.tbContour.Enter += new System.EventHandler(this.tbContour_Enter);
            // 
            // tbOrigin
            // 
            this.tbOrigin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOrigin.Location = new System.Drawing.Point(102, 180);
            this.tbOrigin.Name = "tbOrigin";
            this.tbOrigin.ReadOnly = true;
            this.tbOrigin.Size = new System.Drawing.Size(130, 20);
            this.tbOrigin.TabIndex = 13;
            // 
            // bContour
            // 
            this.bContour.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bContour.Location = new System.Drawing.Point(234, 217);
            this.bContour.Name = "bContour";
            this.bContour.Size = new System.Drawing.Size(21, 21);
            this.bContour.TabIndex = 41;
            this.bContour.TabStop = false;
            this.bContour.Text = "۞";
            this.toolTip1.SetToolTip(this.bContour, "Выбор контура плитки");
            this.bContour.UseVisualStyleBackColor = true;
            this.bContour.Click += new System.EventHandler(this.bProcessingArea_Click);
            // 
            // bOrigin
            // 
            this.bOrigin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bOrigin.Location = new System.Drawing.Point(234, 180);
            this.bOrigin.Name = "bOrigin";
            this.bOrigin.Size = new System.Drawing.Size(21, 21);
            this.bOrigin.TabIndex = 42;
            this.bOrigin.TabStop = false;
            this.bOrigin.Text = "۞";
            this.toolTip1.SetToolTip(this.bOrigin, "Выбор точки начала координат");
            this.bOrigin.UseVisualStyleBackColor = true;
            this.bOrigin.Click += new System.EventHandler(this.bOrigin_Click);
            // 
            // tbPenetrationFeed
            // 
            this.tbPenetrationFeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPenetrationFeed.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessParamsBindingSource, "PenetrationFeed", true));
            this.tbPenetrationFeed.Location = new System.Drawing.Point(102, 157);
            this.tbPenetrationFeed.Name = "tbPenetrationFeed";
            this.tbPenetrationFeed.Size = new System.Drawing.Size(152, 20);
            this.tbPenetrationFeed.TabIndex = 12;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 160);
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
            this.tbFrequency.TabIndex = 3;
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
            // bObjects
            // 
            this.bObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bObjects.Location = new System.Drawing.Point(233, 240);
            this.bObjects.Name = "bObjects";
            this.bObjects.Size = new System.Drawing.Size(21, 21);
            this.bObjects.TabIndex = 48;
            this.bObjects.TabStop = false;
            this.bObjects.Text = "۞";
            this.toolTip1.SetToolTip(this.bObjects, "Выбор 2 элементов в нижнем левом углу плитки.");
            this.bObjects.UseVisualStyleBackColor = true;
            this.bObjects.Click += new System.EventHandler(this.bObjects_Click);
            // 
            // tbObjects
            // 
            this.tbObjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbObjects.Location = new System.Drawing.Point(102, 241);
            this.tbObjects.Name = "tbObjects";
            this.tbObjects.ReadOnly = true;
            this.tbObjects.Size = new System.Drawing.Size(130, 20);
            this.tbObjects.TabIndex = 22;
            this.tbObjects.Enter += new System.EventHandler(this.tbObjects_Enter);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(2, 244);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(103, 13);
            this.label10.TabIndex = 46;
            this.label10.Text = "2 элемента плитки";
            // 
            // tbType
            // 
            this.tbType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbType.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessBindingSource, "Type", true));
            this.tbType.Location = new System.Drawing.Point(102, 279);
            this.tbType.Name = "tbType";
            this.tbType.ReadOnly = true;
            this.tbType.Size = new System.Drawing.Size(152, 20);
            this.tbType.TabIndex = 41;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 282);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 13);
            this.label12.TabIndex = 51;
            this.label12.Text = "Тип плитки";
            // 
            // tbBandStart2
            // 
            this.tbBandStart2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBandStart2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessBindingSource, "BandStart2", true));
            this.tbBandStart2.Location = new System.Drawing.Point(102, 371);
            this.tbBandStart2.Name = "tbBandStart2";
            this.tbBandStart2.Size = new System.Drawing.Size(152, 20);
            this.tbBandStart2.TabIndex = 45;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 374);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(94, 13);
            this.label13.TabIndex = 53;
            this.label13.Text = "Начало полосы 2";
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
            this.Controls.Add(this.tbBandStart2);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.tbType);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.bObjects);
            this.Controls.Add(this.tbObjects);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tbFrequency);
            this.Controls.Add(this.lbFrequency);
            this.Controls.Add(this.tbPenetrationFeed);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.bOrigin);
            this.Controls.Add(this.bContour);
            this.Controls.Add(this.tbOrigin);
            this.Controls.Add(this.tbContour);
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
            this.Controls.Add(this.tbBandStart1);
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
        private System.Windows.Forms.TextBox tbBandStart1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbBandSpacing;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button bTool;
        private System.Windows.Forms.TextBox tbTool;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbContour;
        private System.Windows.Forms.TextBox tbOrigin;
        private System.Windows.Forms.Button bContour;
        private System.Windows.Forms.Button bOrigin;
        private System.Windows.Forms.TextBox tbPenetrationFeed;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.BindingSource tactileTechProcessParamsBindingSource;
        private System.Windows.Forms.TextBox tbFrequency;
        private System.Windows.Forms.Label lbFrequency;
        private System.Windows.Forms.BindingSource tactileTechProcessBindingSource;
        private System.Windows.Forms.Button bObjects;
        private System.Windows.Forms.TextBox tbObjects;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbType;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tbBandStart2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
