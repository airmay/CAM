namespace CAM.UI
{
    partial class PullingView
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
            this.bPulling = new System.Windows.Forms.Button();
            this.gbPulling = new System.Windows.Forms.GroupBox();
            this.cbMove = new System.Windows.Forms.CheckBox();
            this.tbDist = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lPulling = new System.Windows.Forms.Label();
            this.gbPulling.SuspendLayout();
            this.SuspendLayout();
            // 
            // bPulling
            // 
            this.bPulling.Location = new System.Drawing.Point(9, 73);
            this.bPulling.Name = "bPulling";
            this.bPulling.Size = new System.Drawing.Size(75, 23);
            this.bPulling.TabIndex = 2;
            this.bPulling.Text = "Притянуть";
            this.bPulling.UseVisualStyleBackColor = true;
            this.bPulling.Click += new System.EventHandler(this.bPulling_Click);
            // 
            // gbPulling
            // 
            this.gbPulling.Controls.Add(this.lPulling);
            this.gbPulling.Controls.Add(this.cbMove);
            this.gbPulling.Controls.Add(this.tbDist);
            this.gbPulling.Controls.Add(this.label5);
            this.gbPulling.Controls.Add(this.bPulling);
            this.gbPulling.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbPulling.Location = new System.Drawing.Point(10, 10);
            this.gbPulling.Name = "gbPulling";
            this.gbPulling.Size = new System.Drawing.Size(315, 126);
            this.gbPulling.TabIndex = 3;
            this.gbPulling.TabStop = false;
            this.gbPulling.Text = "Притягивание";
            // 
            // cbMove
            // 
            this.cbMove.AutoSize = true;
            this.cbMove.Location = new System.Drawing.Point(9, 50);
            this.cbMove.Name = "cbMove";
            this.cbMove.Size = new System.Drawing.Size(124, 17);
            this.cbMove.TabIndex = 6;
            this.cbMove.Text = "Плавающий режим";
            this.cbMove.UseVisualStyleBackColor = true;
            // 
            // tbDist
            // 
            this.tbDist.Location = new System.Drawing.Point(76, 24);
            this.tbDist.Name = "tbDist";
            this.tbDist.Size = new System.Drawing.Size(57, 20);
            this.tbDist.TabIndex = 5;
            this.tbDist.Text = "5";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Дистанция:";
            // 
            // lPulling
            // 
            this.lPulling.AutoSize = true;
            this.lPulling.ForeColor = System.Drawing.Color.Red;
            this.lPulling.Location = new System.Drawing.Point(6, 99);
            this.lPulling.Name = "lPulling";
            this.lPulling.Size = new System.Drawing.Size(0, 13);
            this.lPulling.TabIndex = 8;
            // 
            // PullingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbPulling);
            this.Name = "PullingView";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(335, 473);
            this.gbPulling.ResumeLayout(false);
            this.gbPulling.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button bPulling;
        private System.Windows.Forms.GroupBox gbPulling;
        private System.Windows.Forms.TextBox tbDist;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbMove;
        private System.Windows.Forms.Label lPulling;
    }
}
