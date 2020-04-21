namespace CAM.Sawing
{
    partial class SawingTechOperationView
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
            this.gbSawingModes = new System.Windows.Forms.GroupBox();
            this.sawingModesView = new CAM.Sawing.SawingModesView();
            this.tbAngleA = new System.Windows.Forms.TextBox();
            this.sawingTechOperationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lbAngleA = new System.Windows.Forms.Label();
            this.bObject = new System.Windows.Forms.Button();
            this.tbObject = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.gbSawingModes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sawingTechOperationBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // gbSawingModes
            // 
            this.gbSawingModes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSawingModes.Controls.Add(this.sawingModesView);
            this.gbSawingModes.Location = new System.Drawing.Point(2, 103);
            this.gbSawingModes.Name = "gbSawingModes";
            this.gbSawingModes.Size = new System.Drawing.Size(252, 434);
            this.gbSawingModes.TabIndex = 50;
            this.gbSawingModes.TabStop = false;
            this.gbSawingModes.Text = "Режим обработки";
            // 
            // sawingModesView
            // 
            this.sawingModesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sawingModesView.Location = new System.Drawing.Point(3, 16);
            this.sawingModesView.Name = "sawingModesView";
            this.sawingModesView.Size = new System.Drawing.Size(246, 415);
            this.sawingModesView.TabIndex = 77;
            // 
            // tbAngleA
            // 
            this.tbAngleA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAngleA.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.sawingTechOperationBindingSource, "AngleA", true));
            this.tbAngleA.Location = new System.Drawing.Point(99, 39);
            this.tbAngleA.Name = "tbAngleA";
            this.tbAngleA.Size = new System.Drawing.Size(152, 20);
            this.tbAngleA.TabIndex = 30;
            // 
            // sawingTechOperationBindingSource
            // 
            this.sawingTechOperationBindingSource.DataSource = typeof(CAM.Sawing.SawingTechOperation);
            // 
            // lbAngleA
            // 
            this.lbAngleA.AutoSize = true;
            this.lbAngleA.Location = new System.Drawing.Point(0, 42);
            this.lbAngleA.Name = "lbAngleA";
            this.lbAngleA.Size = new System.Drawing.Size(59, 13);
            this.lbAngleA.TabIndex = 87;
            this.lbAngleA.Text = "Верт. угол";
            // 
            // bObject
            // 
            this.bObject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bObject.Location = new System.Drawing.Point(231, 65);
            this.bObject.Name = "bObject";
            this.bObject.Size = new System.Drawing.Size(21, 21);
            this.bObject.TabIndex = 45;
            this.bObject.TabStop = false;
            this.bObject.Text = "۞";
            this.bObject.UseVisualStyleBackColor = true;
            this.bObject.Click += new System.EventHandler(this.bObject_Click);
            // 
            // tbObject
            // 
            this.tbObject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbObject.Location = new System.Drawing.Point(99, 65);
            this.tbObject.Name = "tbObject";
            this.tbObject.ReadOnly = true;
            this.tbObject.Size = new System.Drawing.Size(130, 20);
            this.tbObject.TabIndex = 40;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(0, 69);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 85;
            this.label8.Text = "Объект";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.sawingTechOperationBindingSource, "IsExactlyBegin", true));
            this.checkBox1.Location = new System.Drawing.Point(5, 12);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(94, 17);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "Начало точно";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.sawingTechOperationBindingSource, "IsExactlyEnd", true));
            this.checkBox2.Location = new System.Drawing.Point(99, 12);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(88, 17);
            this.checkBox2.TabIndex = 20;
            this.checkBox2.Text = "Конец точно";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // SawingTechOperationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.gbSawingModes);
            this.Controls.Add(this.tbAngleA);
            this.Controls.Add(this.lbAngleA);
            this.Controls.Add(this.bObject);
            this.Controls.Add(this.tbObject);
            this.Controls.Add(this.label8);
            this.Name = "SawingTechOperationView";
            this.Size = new System.Drawing.Size(257, 540);
            this.gbSawingModes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sawingTechOperationBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox gbSawingModes;
        private SawingModesView sawingModesView;
        private System.Windows.Forms.TextBox tbAngleA;
        private System.Windows.Forms.Label lbAngleA;
        private System.Windows.Forms.Button bObject;
        private System.Windows.Forms.TextBox tbObject;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.BindingSource sawingTechOperationBindingSource;
    }
}
