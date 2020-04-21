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
            this.tbFeedMax = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbFeedMin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbZEntry = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbZSafety = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
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
            // tbFeedMax
            // 
            this.tbFeedMax.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFeedMax.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.conesTechOperationBindingSource, "FeedMax", true));
            this.tbFeedMax.Location = new System.Drawing.Point(102, 29);
            this.tbFeedMax.Name = "tbFeedMax";
            this.tbFeedMax.Size = new System.Drawing.Size(152, 20);
            this.tbFeedMax.TabIndex = 50;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 54;
            this.label2.Text = "Подача макс.";
            // 
            // tbFeedMin
            // 
            this.tbFeedMin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFeedMin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.conesTechOperationBindingSource, "FeedMin", true));
            this.tbFeedMin.Location = new System.Drawing.Point(102, 55);
            this.tbFeedMin.Name = "tbFeedMin";
            this.tbFeedMin.Size = new System.Drawing.Size(152, 20);
            this.tbFeedMin.TabIndex = 55;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 56;
            this.label1.Text = "Подача мин.";
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
            this.textBox2.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 58;
            this.label3.Text = "Начало полосы 2";
            this.label3.Visible = false;
            // 
            // tbZEntry
            // 
            this.tbZEntry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbZEntry.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.conesTechOperationBindingSource, "ZEntry", true));
            this.tbZEntry.Location = new System.Drawing.Point(102, 135);
            this.tbZEntry.Name = "tbZEntry";
            this.tbZEntry.Size = new System.Drawing.Size(152, 20);
            this.tbZEntry.TabIndex = 61;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 62;
            this.label4.Text = "Z входа";
            // 
            // tbZSafety
            // 
            this.tbZSafety.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbZSafety.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.conesTechOperationBindingSource, "ZSafety", true));
            this.tbZSafety.Location = new System.Drawing.Point(102, 109);
            this.tbZSafety.Name = "tbZSafety";
            this.tbZSafety.Size = new System.Drawing.Size(152, 20);
            this.tbZSafety.TabIndex = 59;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 60;
            this.label5.Text = "Z безопасности";
            // 
            // ConesTechOperationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbZEntry);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbZSafety);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbFeedMin);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbFeedMax);
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
        private System.Windows.Forms.TextBox tbFeedMax;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbFeedMin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbZEntry;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbZSafety;
        private System.Windows.Forms.Label label5;
    }
}
