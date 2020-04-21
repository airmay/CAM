namespace CAM.Tactile
{
    partial class ChamfersTechOperationView
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
            this.tbFeed = new System.Windows.Forms.TextBox();
            this.chamfersTechOperationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label11 = new System.Windows.Forms.Label();
            this.tbProcessingAngle = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chamfersTechOperationBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tbFeed
            // 
            this.tbFeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFeed.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.chamfersTechOperationBindingSource, "CuttingFeed", true));
            this.tbFeed.Location = new System.Drawing.Point(102, 26);
            this.tbFeed.Name = "tbFeed";
            this.tbFeed.Size = new System.Drawing.Size(152, 20);
            this.tbFeed.TabIndex = 45;
            // 
            // chamfersTechOperationBindingSource
            // 
            this.chamfersTechOperationBindingSource.DataSource = typeof(CAM.Tactile.ChamfersTechOperation);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 29);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 13);
            this.label11.TabIndex = 47;
            this.label11.Text = "Подача";
            // 
            // tbProcessingAngle
            // 
            this.tbProcessingAngle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProcessingAngle.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.chamfersTechOperationBindingSource, "ProcessingAngle", true));
            this.tbProcessingAngle.Location = new System.Drawing.Point(102, 3);
            this.tbProcessingAngle.Name = "tbProcessingAngle";
            this.tbProcessingAngle.Size = new System.Drawing.Size(152, 20);
            this.tbProcessingAngle.TabIndex = 44;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 46;
            this.label5.Text = "Угол полосы";
            // 
            // ChamfersTechOperationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbFeed);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.tbProcessingAngle);
            this.Controls.Add(this.label5);
            this.Name = "ChamfersTechOperationView";
            this.Size = new System.Drawing.Size(257, 517);
            ((System.ComponentModel.ISupportInitialize)(this.chamfersTechOperationBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbFeed;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbProcessingAngle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.BindingSource chamfersTechOperationBindingSource;
    }
}
