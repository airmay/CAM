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
            this.conesTechOperationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lbFrequency = new System.Windows.Forms.Label();
            this.tbProcessingAngle = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.conesTechOperationBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tbFrequency
            // 
            this.tbFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFrequency.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.conesTechOperationBindingSource, "Frequency", true));
            this.tbFrequency.Location = new System.Drawing.Point(102, 3);
            this.tbFrequency.Name = "tbFrequency";
            this.tbFrequency.Size = new System.Drawing.Size(152, 20);
            this.tbFrequency.TabIndex = 46;
            // 
            // conesTechOperationBindingSource
            // 
            this.conesTechOperationBindingSource.DataSource = typeof(CAM.Tactile.ConesTechOperation);
            // 
            // lbFrequency
            // 
            this.lbFrequency.AutoSize = true;
            this.lbFrequency.Location = new System.Drawing.Point(3, 6);
            this.lbFrequency.Name = "lbFrequency";
            this.lbFrequency.Size = new System.Drawing.Size(58, 13);
            this.lbFrequency.TabIndex = 50;
            this.lbFrequency.Text = "Шпиндель";
            // 
            // tbProcessingAngle
            // 
            this.tbProcessingAngle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProcessingAngle.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.conesTechOperationBindingSource, "ProcessingAngle", true));
            this.tbProcessingAngle.Location = new System.Drawing.Point(102, 29);
            this.tbProcessingAngle.Name = "tbProcessingAngle";
            this.tbProcessingAngle.Size = new System.Drawing.Size(152, 20);
            this.tbProcessingAngle.TabIndex = 50;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 54;
            this.label2.Text = "Угол полосы";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.conesTechOperationBindingSource, "BandStart1", true));
            this.textBox1.Location = new System.Drawing.Point(102, 55);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(152, 20);
            this.textBox1.TabIndex = 55;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 56;
            this.label1.Text = "Начало полосы 1";
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.conesTechOperationBindingSource, "BandStart2", true));
            this.textBox2.Location = new System.Drawing.Point(102, 81);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(152, 20);
            this.textBox2.TabIndex = 57;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 58;
            this.label3.Text = "Начало полосы 2";
            // 
            // ConesTechOperationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbProcessingAngle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbFrequency);
            this.Controls.Add(this.lbFrequency);
            this.Name = "ConesTechOperationView";
            this.Size = new System.Drawing.Size(257, 517);
            ((System.ComponentModel.ISupportInitialize)(this.conesTechOperationBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbFrequency;
        private System.Windows.Forms.Label lbFrequency;
        private System.Windows.Forms.BindingSource conesTechOperationBindingSource;
        private System.Windows.Forms.TextBox tbProcessingAngle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
    }
}
