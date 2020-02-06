namespace CAM.Tactile
{
    partial class ConesTechOperationView
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
            this.tbFrequency = new System.Windows.Forms.TextBox();
            this.lbFrequency = new System.Windows.Forms.Label();
            this.tbTool = new System.Windows.Forms.TextBox();
            this.bTool = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.conesTechOperationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.conesTechOperationBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tbFrequency
            // 
            this.tbFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFrequency.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.conesTechOperationBindingSource, "Frequency", true));
            this.tbFrequency.Location = new System.Drawing.Point(102, 26);
            this.tbFrequency.Name = "tbFrequency";
            this.tbFrequency.Size = new System.Drawing.Size(152, 20);
            this.tbFrequency.TabIndex = 46;
            // 
            // lbFrequency
            // 
            this.lbFrequency.AutoSize = true;
            this.lbFrequency.Location = new System.Drawing.Point(3, 29);
            this.lbFrequency.Name = "lbFrequency";
            this.lbFrequency.Size = new System.Drawing.Size(58, 13);
            this.lbFrequency.TabIndex = 50;
            this.lbFrequency.Text = "Шпиндель";
            // 
            // tbTool
            // 
            this.tbTool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTool.Enabled = false;
            this.tbTool.ForeColor = System.Drawing.SystemColors.WindowText;
            this.tbTool.Location = new System.Drawing.Point(102, 3);
            this.tbTool.Name = "tbTool";
            this.tbTool.Size = new System.Drawing.Size(130, 20);
            this.tbTool.TabIndex = 49;
            // 
            // bTool
            // 
            this.bTool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bTool.Location = new System.Drawing.Point(234, 3);
            this.bTool.Name = "bTool";
            this.bTool.Size = new System.Drawing.Size(20, 20);
            this.bTool.TabIndex = 48;
            this.bTool.TabStop = false;
            this.bTool.Text = "Ξ";
            this.bTool.UseVisualStyleBackColor = true;
            this.bTool.Click += new System.EventHandler(this.bTool_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 47;
            this.label6.Text = "Фреза";
            // 
            // conesTechOperationBindingSource
            // 
            this.conesTechOperationBindingSource.DataSource = typeof(CAM.Tactile.ConesTechOperation);
            // 
            // ConesTechOperationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbFrequency);
            this.Controls.Add(this.lbFrequency);
            this.Controls.Add(this.tbTool);
            this.Controls.Add(this.bTool);
            this.Controls.Add(this.label6);
            this.Name = "ConesTechOperationView";
            this.Size = new System.Drawing.Size(257, 517);
            ((System.ComponentModel.ISupportInitialize)(this.conesTechOperationBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbFrequency;
        private System.Windows.Forms.Label lbFrequency;
        private System.Windows.Forms.TextBox tbTool;
        private System.Windows.Forms.Button bTool;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.BindingSource conesTechOperationBindingSource;
    }
}
