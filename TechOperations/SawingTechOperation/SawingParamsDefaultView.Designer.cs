namespace CAM.UI
{
    partial class SawingParamsDefaultView
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbTrajectoryType = new System.Windows.Forms.ComboBox();
            this.lbTrajectoryType = new System.Windows.Forms.Label();
            this.sawingModesView = new CAM.UI.SawingModesView();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbTrajectoryType);
            this.panel1.Controls.Add(this.lbTrajectoryType);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(358, 29);
            this.panel1.TabIndex = 7;
            // 
            // cbTrajectoryType
            // 
            this.cbTrajectoryType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTrajectoryType.FormattingEnabled = true;
            this.cbTrajectoryType.Items.AddRange(new object[] {
            "Прямолинейная",
            "Криволинейная"});
            this.cbTrajectoryType.Location = new System.Drawing.Point(91, 3);
            this.cbTrajectoryType.Name = "cbTrajectoryType";
            this.cbTrajectoryType.Size = new System.Drawing.Size(257, 21);
            this.cbTrajectoryType.TabIndex = 1;
            this.cbTrajectoryType.SelectedIndexChanged += new System.EventHandler(this.cbTrajectoryType_SelectedIndexChanged);
            // 
            // lbTrajectoryType
            // 
            this.lbTrajectoryType.AutoSize = true;
            this.lbTrajectoryType.Location = new System.Drawing.Point(3, 6);
            this.lbTrajectoryType.Name = "lbTrajectoryType";
            this.lbTrajectoryType.Size = new System.Drawing.Size(67, 13);
            this.lbTrajectoryType.TabIndex = 0;
            this.lbTrajectoryType.Text = "Траектория";
            // 
            // sawingModesView
            // 
            this.sawingModesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sawingModesView.Location = new System.Drawing.Point(0, 29);
            this.sawingModesView.Name = "sawingModesView";
            this.sawingModesView.Size = new System.Drawing.Size(358, 594);
            this.sawingModesView.TabIndex = 0;
            // 
            // SawingParamsDefaultView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sawingModesView);
            this.Controls.Add(this.panel1);
            this.Name = "SawingParamsDefaultView";
            this.Size = new System.Drawing.Size(358, 623);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbTrajectoryType;
        private SawingModesView sawingModesView;
        private System.Windows.Forms.ComboBox cbTrajectoryType;
    }
}
