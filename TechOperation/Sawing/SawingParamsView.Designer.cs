namespace CAM.TechOperation.Sawing
{
    partial class SawingParamsView
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
            this.edCompensation = new System.Windows.Forms.TextBox();
            this.lbCompensation = new System.Windows.Forms.Label();
            this.gbSawingModes = new System.Windows.Forms.GroupBox();
            this.sawingModesView = new SawingModesView();
            this.pSawingModesButton = new System.Windows.Forms.Panel();
            this.bLoadSawingModes = new System.Windows.Forms.Button();
            this.bSaveSawingModes = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.sawingParamsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gbSawingModes.SuspendLayout();
            this.pSawingModesButton.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sawingParamsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // edCompensation
            // 
            this.edCompensation.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.sawingParamsBindingSource, "Compensation", true));
            this.edCompensation.Enabled = false;
            this.edCompensation.Location = new System.Drawing.Point(82, 7);
            this.edCompensation.Name = "edCompensation";
            this.edCompensation.Size = new System.Drawing.Size(50, 20);
            this.edCompensation.TabIndex = 5;
            // 
            // lbCompensation
            // 
            this.lbCompensation.AutoSize = true;
            this.lbCompensation.Location = new System.Drawing.Point(0, 10);
            this.lbCompensation.Name = "lbCompensation";
            this.lbCompensation.Size = new System.Drawing.Size(76, 13);
            this.lbCompensation.TabIndex = 4;
            this.lbCompensation.Text = "Компенсация";
            // 
            // gbSawingModes
            // 
            this.gbSawingModes.Controls.Add(this.sawingModesView);
            this.gbSawingModes.Controls.Add(this.pSawingModesButton);
            this.gbSawingModes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbSawingModes.Location = new System.Drawing.Point(0, 34);
            this.gbSawingModes.Name = "gbSawingModes";
            this.gbSawingModes.Size = new System.Drawing.Size(245, 377);
            this.gbSawingModes.TabIndex = 2;
            this.gbSawingModes.TabStop = false;
            this.gbSawingModes.Text = "Режимы обработки";
            // 
            // sawingModesView
            // 
            this.sawingModesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sawingModesView.Location = new System.Drawing.Point(3, 16);
            this.sawingModesView.Name = "sawingModesView";
            this.sawingModesView.Size = new System.Drawing.Size(239, 329);
            this.sawingModesView.TabIndex = 1;
            // 
            // pSawingModesButton
            // 
            this.pSawingModesButton.Controls.Add(this.bLoadSawingModes);
            this.pSawingModesButton.Controls.Add(this.bSaveSawingModes);
            this.pSawingModesButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pSawingModesButton.Location = new System.Drawing.Point(3, 345);
            this.pSawingModesButton.Name = "pSawingModesButton";
            this.pSawingModesButton.Size = new System.Drawing.Size(239, 29);
            this.pSawingModesButton.TabIndex = 2;
            this.pSawingModesButton.Visible = false;
            // 
            // bLoadSawingModes
            // 
            this.bLoadSawingModes.Location = new System.Drawing.Point(84, 3);
            this.bLoadSawingModes.Name = "bLoadSawingModes";
            this.bLoadSawingModes.Size = new System.Drawing.Size(75, 23);
            this.bLoadSawingModes.TabIndex = 1;
            this.bLoadSawingModes.Text = "Загрузить";
            this.bLoadSawingModes.UseVisualStyleBackColor = true;
            // 
            // bSaveSawingModes
            // 
            this.bSaveSawingModes.Location = new System.Drawing.Point(3, 3);
            this.bSaveSawingModes.Name = "bSaveSawingModes";
            this.bSaveSawingModes.Size = new System.Drawing.Size(75, 23);
            this.bSaveSawingModes.TabIndex = 0;
            this.bSaveSawingModes.Text = "Сохранить";
            this.bSaveSawingModes.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.edCompensation);
            this.panel1.Controls.Add(this.lbCompensation);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(245, 34);
            this.panel1.TabIndex = 7;
            // 
            // sawingParamsBindingSource
            // 
            this.sawingParamsBindingSource.DataSource = typeof(SawingTechOperationParams);
            // 
            // SawingParamsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbSawingModes);
            this.Controls.Add(this.panel1);
            this.Name = "SawingParamsView";
            this.Size = new System.Drawing.Size(245, 411);
            this.gbSawingModes.ResumeLayout(false);
            this.pSawingModesButton.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sawingParamsBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private SawingModesView sawingModesView;
        private System.Windows.Forms.TextBox edCompensation;
        private System.Windows.Forms.Label lbCompensation;
        private System.Windows.Forms.GroupBox gbSawingModes;
        private System.Windows.Forms.Panel pSawingModesButton;
        private System.Windows.Forms.Button bLoadSawingModes;
        private System.Windows.Forms.Button bSaveSawingModes;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.BindingSource sawingParamsBindingSource;
	}
}
