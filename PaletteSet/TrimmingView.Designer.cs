namespace CAM
{
    partial class TrimmingView
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbItemsCount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bObjects = new System.Windows.Forms.Button();
            this.tbObjects = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tbItemsCount);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.bObjects);
            this.groupBox1.Controls.Add(this.tbObjects);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(241, 69);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Подрезка изображения";
            // 
            // tbItemsCount
            // 
            this.tbItemsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbItemsCount.Location = new System.Drawing.Point(89, 40);
            this.tbItemsCount.Name = "tbItemsCount";
            this.tbItemsCount.ReadOnly = true;
            this.tbItemsCount.Size = new System.Drawing.Size(119, 20);
            this.tbItemsCount.TabIndex = 49;
            this.tbItemsCount.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 48;
            this.label2.Text = "Контуров";
            // 
            // bObjects
            // 
            this.bObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bObjects.Location = new System.Drawing.Point(209, 17);
            this.bObjects.Name = "bObjects";
            this.bObjects.Size = new System.Drawing.Size(21, 21);
            this.bObjects.TabIndex = 47;
            this.bObjects.TabStop = false;
            this.bObjects.Text = "۞";
            this.bObjects.UseVisualStyleBackColor = true;
            this.bObjects.Click += new System.EventHandler(this.bObjects_Click);
            // 
            // tbObjects
            // 
            this.tbObjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbObjects.Location = new System.Drawing.Point(89, 17);
            this.tbObjects.Name = "tbObjects";
            this.tbObjects.ReadOnly = true;
            this.tbObjects.Size = new System.Drawing.Size(119, 20);
            this.tbObjects.TabIndex = 46;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 45;
            this.label1.Text = "Контур";
            // 
            // TrimmingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "TrimmingView";
            this.Size = new System.Drawing.Size(247, 78);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bObjects;
        private System.Windows.Forms.TextBox tbObjects;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbItemsCount;
        private System.Windows.Forms.Label label2;
    }
}
