namespace CAM.Tactile
{
    partial class MeasurementTechOperationView
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
            this.bSelectPoints = new System.Windows.Forms.Button();
            this.tbPointsCount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tactileTechProcessBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.rbAverage = new System.Windows.Forms.RadioButton();
            this.rbMinimum = new System.Windows.Forms.RadioButton();
            this.gbCalcMethodType = new System.Windows.Forms.GroupBox();
            this.rbСorners = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.tactileTechProcessBindingSource)).BeginInit();
            this.gbCalcMethodType.SuspendLayout();
            this.SuspendLayout();
            // 
            // bSelectPoints
            // 
            this.bSelectPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bSelectPoints.Location = new System.Drawing.Point(232, 3);
            this.bSelectPoints.Name = "bSelectPoints";
            this.bSelectPoints.Size = new System.Drawing.Size(21, 21);
            this.bSelectPoints.TabIndex = 45;
            this.bSelectPoints.TabStop = false;
            this.bSelectPoints.Text = "۞";
            this.bSelectPoints.UseVisualStyleBackColor = true;
            this.bSelectPoints.Click += new System.EventHandler(this.bSelectPoints_Click);
            // 
            // tbPointsCount
            // 
            this.tbPointsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPointsCount.Location = new System.Drawing.Point(102, 3);
            this.tbPointsCount.Name = "tbPointsCount";
            this.tbPointsCount.ReadOnly = true;
            this.tbPointsCount.Size = new System.Drawing.Size(128, 20);
            this.tbPointsCount.TabIndex = 43;
            this.tbPointsCount.Enter += new System.EventHandler(this.tbPointsCount_Enter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 44;
            this.label1.Text = "Точки";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tactileTechProcessBindingSource, "Thickness", true));
            this.textBox1.Location = new System.Drawing.Point(102, 27);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(128, 20);
            this.textBox1.TabIndex = 46;
            // 
            // tactileTechProcessBindingSource
            // 
            this.tactileTechProcessBindingSource.DataSource = typeof(CAM.Tactile.TactileTechProcess);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 47;
            this.label2.Text = "Толщина";
            // 
            // rbAverage
            // 
            this.rbAverage.AutoSize = true;
            this.rbAverage.Checked = true;
            this.rbAverage.Location = new System.Drawing.Point(18, 19);
            this.rbAverage.Name = "rbAverage";
            this.rbAverage.Size = new System.Drawing.Size(68, 17);
            this.rbAverage.TabIndex = 48;
            this.rbAverage.TabStop = true;
            this.rbAverage.Text = "Среднее";
            this.rbAverage.UseVisualStyleBackColor = true;
            this.rbAverage.CheckedChanged += new System.EventHandler(this.rbCalcMethodChanged);
            // 
            // rbMinimum
            // 
            this.rbMinimum.AutoSize = true;
            this.rbMinimum.Location = new System.Drawing.Point(18, 42);
            this.rbMinimum.Name = "rbMinimum";
            this.rbMinimum.Size = new System.Drawing.Size(91, 17);
            this.rbMinimum.TabIndex = 49;
            this.rbMinimum.Text = "Наименьшее";
            this.rbMinimum.UseVisualStyleBackColor = true;
            this.rbMinimum.CheckedChanged += new System.EventHandler(this.rbCalcMethodChanged);
            // 
            // gbCalcMethodType
            // 
            this.gbCalcMethodType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbCalcMethodType.Controls.Add(this.rbСorners);
            this.gbCalcMethodType.Controls.Add(this.rbMinimum);
            this.gbCalcMethodType.Controls.Add(this.rbAverage);
            this.gbCalcMethodType.Location = new System.Drawing.Point(4, 53);
            this.gbCalcMethodType.Name = "gbCalcMethodType";
            this.gbCalcMethodType.Size = new System.Drawing.Size(226, 93);
            this.gbCalcMethodType.TabIndex = 50;
            this.gbCalcMethodType.TabStop = false;
            this.gbCalcMethodType.Text = "Метод расчета";
            // 
            // rbСorners
            // 
            this.rbСorners.AutoSize = true;
            this.rbСorners.Location = new System.Drawing.Point(18, 65);
            this.rbСorners.Name = "rbСorners";
            this.rbСorners.Size = new System.Drawing.Size(81, 17);
            this.rbСorners.TabIndex = 50;
            this.rbСorners.Text = "По 4 углам";
            this.rbСorners.UseVisualStyleBackColor = true;
            this.rbСorners.CheckedChanged += new System.EventHandler(this.rbCalcMethodChanged);
            // 
            // MeasurementTechOperationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbCalcMethodType);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bSelectPoints);
            this.Controls.Add(this.tbPointsCount);
            this.Controls.Add(this.label1);
            this.Name = "MeasurementTechOperationView";
            this.Size = new System.Drawing.Size(255, 366);
            ((System.ComponentModel.ISupportInitialize)(this.tactileTechProcessBindingSource)).EndInit();
            this.gbCalcMethodType.ResumeLayout(false);
            this.gbCalcMethodType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bSelectPoints;
        private System.Windows.Forms.TextBox tbPointsCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.BindingSource tactileTechProcessBindingSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rbAverage;
        private System.Windows.Forms.RadioButton rbMinimum;
        private System.Windows.Forms.GroupBox gbCalcMethodType;
        private System.Windows.Forms.RadioButton rbСorners;
    }
}
