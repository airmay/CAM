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
            this.bSelectPoints = new System.Windows.Forms.Button();
            this.tbPointsCount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
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
            this.tbPointsCount.Location = new System.Drawing.Point(100, 3);
            this.tbPointsCount.Name = "tbPointsCount";
            this.tbPointsCount.ReadOnly = true;
            this.tbPointsCount.Size = new System.Drawing.Size(130, 20);
            this.tbPointsCount.TabIndex = 43;
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
            // MeasurementTechOperationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bSelectPoints);
            this.Controls.Add(this.tbPointsCount);
            this.Controls.Add(this.label1);
            this.Name = "MeasurementTechOperationView";
            this.Size = new System.Drawing.Size(255, 366);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bSelectPoints;
        private System.Windows.Forms.TextBox tbPointsCount;
        private System.Windows.Forms.Label label1;
    }
}
