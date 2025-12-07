namespace CAM.UtilViews
{
    partial class UtilsView
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
            this.pullingView1 = new PullingView();
            this.trimmingView1 = new TrimmingView();
            this.SuspendLayout();
            // 
            // pullingView1
            // 
            this.pullingView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pullingView1.Location = new System.Drawing.Point(0, 0);
            this.pullingView1.Name = "pullingView1";
            this.pullingView1.Padding = new System.Windows.Forms.Padding(3);
            this.pullingView1.Size = new System.Drawing.Size(362, 121);
            this.pullingView1.TabIndex = 0;
            // 
            // trimmingView1
            // 
            this.trimmingView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.trimmingView1.Location = new System.Drawing.Point(0, 121);
            this.trimmingView1.Name = "trimmingView1";
            this.trimmingView1.Size = new System.Drawing.Size(362, 119);
            this.trimmingView1.TabIndex = 1;
            // 
            // UtilsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.trimmingView1);
            this.Controls.Add(this.pullingView1);
            this.Name = "UtilsView";
            this.Size = new System.Drawing.Size(362, 600);
            this.ResumeLayout(false);

        }

        #endregion

        private PullingView pullingView1;
        private TrimmingView trimmingView1;
    }
}
