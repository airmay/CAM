namespace CAM.TechOperation
{
    partial class ParamsForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pParams = new System.Windows.Forms.Panel();
            this.pClose = new System.Windows.Forms.Panel();
            this.bClose = new System.Windows.Forms.Button();
            this.pClose.SuspendLayout();
            this.SuspendLayout();
            // 
            // pParams
            // 
            this.pParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pParams.Location = new System.Drawing.Point(0, 0);
            this.pParams.Name = "pParams";
            this.pParams.Size = new System.Drawing.Size(323, 517);
            this.pParams.TabIndex = 0;
            // 
            // pClose
            // 
            this.pClose.AutoSize = true;
            this.pClose.Controls.Add(this.bClose);
            this.pClose.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pClose.Location = new System.Drawing.Point(0, 488);
            this.pClose.Name = "pClose";
            this.pClose.Size = new System.Drawing.Size(323, 29);
            this.pClose.TabIndex = 1;
            // 
            // bClose
            // 
            this.bClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bClose.Location = new System.Drawing.Point(236, 3);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(75, 23);
            this.bClose.TabIndex = 0;
            this.bClose.Text = "Закрыть";
            this.bClose.UseVisualStyleBackColor = true;
            // 
            // ParamsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bClose;
            this.ClientSize = new System.Drawing.Size(323, 517);
            this.Controls.Add(this.pClose);
            this.Controls.Add(this.pParams);
            this.Name = "ParamsForm";
            this.Text = "ParamsForm";
            this.pClose.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pParams;
        private System.Windows.Forms.Panel pClose;
        private System.Windows.Forms.Button bClose;
    }
}