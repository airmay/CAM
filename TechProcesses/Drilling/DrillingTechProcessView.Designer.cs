namespace CAM.TechProcesses.Drilling
{
    partial class DrillingTechProcessView
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
            this.bObjects = new System.Windows.Forms.Button();
            this.tbObjects = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbFrequency = new System.Windows.Forms.TextBox();
            this.lbFrequency = new System.Windows.Forms.Label();
            this.tbDepth = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbZEntry = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbZSafety = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbFeedMin = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbFeedMax = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.drillingTechProcessBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.bOrigin = new System.Windows.Forms.Button();
            this.tbOrigin = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.drillingTechProcessBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // bObjects
            // 
            this.bObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bObjects.Location = new System.Drawing.Point(233, 91);
            this.bObjects.Name = "bObjects";
            this.bObjects.Size = new System.Drawing.Size(21, 21);
            this.bObjects.TabIndex = 57;
            this.bObjects.TabStop = false;
            this.bObjects.Text = "۞";
            this.bObjects.UseVisualStyleBackColor = true;
            this.bObjects.Click += new System.EventHandler(this.bObjects_Click);
            // 
            // tbObjects
            // 
            this.tbObjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbObjects.Location = new System.Drawing.Point(102, 92);
            this.tbObjects.Name = "tbObjects";
            this.tbObjects.ReadOnly = true;
            this.tbObjects.Size = new System.Drawing.Size(130, 20);
            this.tbObjects.TabIndex = 40;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(2, 95);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(61, 13);
            this.label10.TabIndex = 56;
            this.label10.Text = "Отверстия";
            // 
            // tbFrequency
            // 
            this.tbFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFrequency.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.drillingTechProcessBindingSource, "Frequency", true));
            this.tbFrequency.Location = new System.Drawing.Point(102, 3);
            this.tbFrequency.Name = "tbFrequency";
            this.tbFrequency.Size = new System.Drawing.Size(152, 20);
            this.tbFrequency.TabIndex = 10;
            // 
            // lbFrequency
            // 
            this.lbFrequency.AutoSize = true;
            this.lbFrequency.Location = new System.Drawing.Point(3, 6);
            this.lbFrequency.Name = "lbFrequency";
            this.lbFrequency.Size = new System.Drawing.Size(58, 13);
            this.lbFrequency.TabIndex = 55;
            this.lbFrequency.Text = "Шпиндель";
            // 
            // tbDepth
            // 
            this.tbDepth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDepth.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.drillingTechProcessBindingSource, "Depth", true));
            this.tbDepth.Location = new System.Drawing.Point(102, 29);
            this.tbDepth.Name = "tbDepth";
            this.tbDepth.Size = new System.Drawing.Size(152, 20);
            this.tbDepth.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 53;
            this.label4.Text = "Глубина";
            // 
            // tbZEntry
            // 
            this.tbZEntry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbZEntry.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.drillingTechProcessBindingSource, "ZEntry", true));
            this.tbZEntry.Location = new System.Drawing.Point(102, 218);
            this.tbZEntry.Name = "tbZEntry";
            this.tbZEntry.Size = new System.Drawing.Size(152, 20);
            this.tbZEntry.TabIndex = 80;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 221);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 72;
            this.label1.Text = "Z входа";
            // 
            // tbZSafety
            // 
            this.tbZSafety.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbZSafety.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.drillingTechProcessBindingSource, "ZSafety", true));
            this.tbZSafety.Location = new System.Drawing.Point(102, 192);
            this.tbZSafety.Name = "tbZSafety";
            this.tbZSafety.Size = new System.Drawing.Size(152, 20);
            this.tbZSafety.TabIndex = 70;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 195);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 70;
            this.label5.Text = "Z безопасности";
            // 
            // tbFeedMin
            // 
            this.tbFeedMin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFeedMin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.drillingTechProcessBindingSource, "FeedMin", true));
            this.tbFeedMin.Location = new System.Drawing.Point(102, 156);
            this.tbFeedMin.Name = "tbFeedMin";
            this.tbFeedMin.Size = new System.Drawing.Size(152, 20);
            this.tbFeedMin.TabIndex = 60;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 66;
            this.label2.Text = "Подача мин.";
            // 
            // tbFeedMax
            // 
            this.tbFeedMax.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFeedMax.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.drillingTechProcessBindingSource, "FeedMax", true));
            this.tbFeedMax.Location = new System.Drawing.Point(102, 130);
            this.tbFeedMax.Name = "tbFeedMax";
            this.tbFeedMax.Size = new System.Drawing.Size(152, 20);
            this.tbFeedMax.TabIndex = 50;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 133);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 13);
            this.label6.TabIndex = 64;
            this.label6.Text = "Подача макс.";
            // 
            // drillingTechProcessBindingSource
            // 
            this.drillingTechProcessBindingSource.DataSource = typeof(CAM.TechProcesses.Drilling.DrillingTechProcess);
            // 
            // bOrigin
            // 
            this.bOrigin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bOrigin.Location = new System.Drawing.Point(234, 65);
            this.bOrigin.Name = "bOrigin";
            this.bOrigin.Size = new System.Drawing.Size(21, 21);
            this.bOrigin.TabIndex = 83;
            this.bOrigin.TabStop = false;
            this.bOrigin.Text = "۞";
            this.bOrigin.UseVisualStyleBackColor = true;
            this.bOrigin.Click += new System.EventHandler(this.bOrigin_Click);
            // 
            // tbOrigin
            // 
            this.tbOrigin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOrigin.Location = new System.Drawing.Point(102, 65);
            this.tbOrigin.Name = "tbOrigin";
            this.tbOrigin.ReadOnly = true;
            this.tbOrigin.Size = new System.Drawing.Size(130, 20);
            this.tbOrigin.TabIndex = 35;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 69);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 13);
            this.label8.TabIndex = 82;
            this.label8.Text = "Начало координат";
            // 
            // DrillingTechProcessView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bOrigin);
            this.Controls.Add(this.tbOrigin);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tbZEntry);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbZSafety);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbFeedMin);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbFeedMax);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.bObjects);
            this.Controls.Add(this.tbObjects);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tbFrequency);
            this.Controls.Add(this.lbFrequency);
            this.Controls.Add(this.tbDepth);
            this.Controls.Add(this.label4);
            this.Name = "DrillingTechProcessView";
            this.Size = new System.Drawing.Size(257, 540);
            ((System.ComponentModel.ISupportInitialize)(this.drillingTechProcessBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bObjects;
        private System.Windows.Forms.TextBox tbObjects;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbFrequency;
        private System.Windows.Forms.Label lbFrequency;
        private System.Windows.Forms.TextBox tbDepth;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbZEntry;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbZSafety;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbFeedMin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbFeedMax;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.BindingSource drillingTechProcessBindingSource;
        private System.Windows.Forms.Button bOrigin;
        private System.Windows.Forms.TextBox tbOrigin;
        private System.Windows.Forms.Label label8;
    }
}
