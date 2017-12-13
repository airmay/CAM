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
            this.lbPenetrationRate = new System.Windows.Forms.Label();
            this.edPenetrationRate = new System.Windows.Forms.TextBox();
            this.tcSawingModes = new System.Windows.Forms.TabControl();
            this.tpSawingLine = new System.Windows.Forms.TabPage();
            this.tpSawingArc = new System.Windows.Forms.TabPage();
            this.sawingModesLineView = new CAM.UI.SawingModesView();
            this.sawingModesArcView = new CAM.UI.SawingModesView();
            this.panel1.SuspendLayout();
            this.tcSawingModes.SuspendLayout();
            this.tpSawingLine.SuspendLayout();
            this.tpSawingArc.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.edPenetrationRate);
            this.panel1.Controls.Add(this.lbPenetrationRate);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(358, 70);
            this.panel1.TabIndex = 7;
            // 
            // lbPenetrationRate
            // 
            this.lbPenetrationRate.AutoSize = true;
            this.lbPenetrationRate.Location = new System.Drawing.Point(3, 6);
            this.lbPenetrationRate.Name = "lbPenetrationRate";
            this.lbPenetrationRate.Size = new System.Drawing.Size(90, 13);
            this.lbPenetrationRate.TabIndex = 0;
            this.lbPenetrationRate.Text = "Скорость малая";
            // 
            // edPenetrationRate
            // 
            this.edPenetrationRate.Location = new System.Drawing.Point(99, 3);
            this.edPenetrationRate.Name = "edPenetrationRate";
            this.edPenetrationRate.Size = new System.Drawing.Size(100, 20);
            this.edPenetrationRate.TabIndex = 1;
            // 
            // tcSawingModes
            // 
            this.tcSawingModes.Controls.Add(this.tpSawingLine);
            this.tcSawingModes.Controls.Add(this.tpSawingArc);
            this.tcSawingModes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcSawingModes.Location = new System.Drawing.Point(0, 70);
            this.tcSawingModes.Name = "tcSawingModes";
            this.tcSawingModes.SelectedIndex = 0;
            this.tcSawingModes.Size = new System.Drawing.Size(358, 553);
            this.tcSawingModes.TabIndex = 8;
            // 
            // tpSawingLine
            // 
            this.tpSawingLine.Controls.Add(this.sawingModesLineView);
            this.tpSawingLine.Location = new System.Drawing.Point(4, 22);
            this.tpSawingLine.Name = "tpSawingLine";
            this.tpSawingLine.Padding = new System.Windows.Forms.Padding(3);
            this.tpSawingLine.Size = new System.Drawing.Size(350, 527);
            this.tpSawingLine.TabIndex = 0;
            this.tpSawingLine.Text = "Прямая";
            this.tpSawingLine.UseVisualStyleBackColor = true;
            // 
            // tpSawingArc
            // 
            this.tpSawingArc.Controls.Add(this.sawingModesArcView);
            this.tpSawingArc.Location = new System.Drawing.Point(4, 22);
            this.tpSawingArc.Name = "tpSawingArc";
            this.tpSawingArc.Padding = new System.Windows.Forms.Padding(3);
            this.tpSawingArc.Size = new System.Drawing.Size(350, 527);
            this.tpSawingArc.TabIndex = 1;
            this.tpSawingArc.Text = "Дуга";
            this.tpSawingArc.UseVisualStyleBackColor = true;
            // 
            // sawingModesLineView
            // 
            this.sawingModesLineView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sawingModesLineView.Location = new System.Drawing.Point(3, 3);
            this.sawingModesLineView.Name = "sawingModesLineView";
            this.sawingModesLineView.Size = new System.Drawing.Size(344, 521);
            this.sawingModesLineView.TabIndex = 0;
            // 
            // sawingModesArcView
            // 
            this.sawingModesArcView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sawingModesArcView.Location = new System.Drawing.Point(3, 3);
            this.sawingModesArcView.Name = "sawingModesArcView";
            this.sawingModesArcView.Size = new System.Drawing.Size(344, 521);
            this.sawingModesArcView.TabIndex = 0;
            // 
            // SawingParamsDefaultView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcSawingModes);
            this.Controls.Add(this.panel1);
            this.Name = "SawingParamsDefaultView";
            this.Size = new System.Drawing.Size(358, 623);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tcSawingModes.ResumeLayout(false);
            this.tpSawingLine.ResumeLayout(false);
            this.tpSawingArc.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox edPenetrationRate;
        private System.Windows.Forms.Label lbPenetrationRate;
        private System.Windows.Forms.TabControl tcSawingModes;
        private System.Windows.Forms.TabPage tpSawingLine;
        private SawingModesView sawingModesLineView;
        private System.Windows.Forms.TabPage tpSawingArc;
        private SawingModesView sawingModesArcView;
    }
}
