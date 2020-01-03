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
            this.gbObjects = new System.Windows.Forms.GroupBox();
            this.bSelectSide = new System.Windows.Forms.Button();
            this.bSetSide = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lbSide = new System.Windows.Forms.Label();
            this.bSelectObjects = new System.Windows.Forms.Button();
            this.bSetObjects = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbObjects = new System.Windows.Forms.Label();
            this.gbGuide = new System.Windows.Forms.GroupBox();
            this.bSelectGuide = new System.Windows.Forms.Button();
            this.bSetGuide = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lbGuide = new System.Windows.Forms.Label();
            this.bPulling = new System.Windows.Forms.Button();
            this.gbPulling = new System.Windows.Forms.GroupBox();
            this.tbDist = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.gbObjects.SuspendLayout();
            this.gbGuide.SuspendLayout();
            this.gbPulling.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbObjects
            // 
            this.gbObjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbObjects.Controls.Add(this.bSelectSide);
            this.gbObjects.Controls.Add(this.bSetSide);
            this.gbObjects.Controls.Add(this.label2);
            this.gbObjects.Controls.Add(this.lbSide);
            this.gbObjects.Controls.Add(this.bSelectObjects);
            this.gbObjects.Controls.Add(this.bSetObjects);
            this.gbObjects.Controls.Add(this.label1);
            this.gbObjects.Controls.Add(this.lbObjects);
            this.gbObjects.Location = new System.Drawing.Point(13, 13);
            this.gbObjects.Name = "gbObjects";
            this.gbObjects.Size = new System.Drawing.Size(308, 124);
            this.gbObjects.TabIndex = 0;
            this.gbObjects.TabStop = false;
            this.gbObjects.Text = "Что притягивать";
            // 
            // bSelectSide
            // 
            this.bSelectSide.Location = new System.Drawing.Point(148, 88);
            this.bSelectSide.Name = "bSelectSide";
            this.bSelectSide.Size = new System.Drawing.Size(75, 23);
            this.bSelectSide.TabIndex = 7;
            this.bSelectSide.Text = "Выделить";
            this.bSelectSide.UseVisualStyleBackColor = true;
            this.bSelectSide.Visible = false;
            this.bSelectSide.Click += new System.EventHandler(this.bSelectSide_Click);
            // 
            // bSetSide
            // 
            this.bSetSide.Location = new System.Drawing.Point(9, 88);
            this.bSetSide.Name = "bSetSide";
            this.bSetSide.Size = new System.Drawing.Size(75, 23);
            this.bSetSide.TabIndex = 6;
            this.bSetSide.Text = "Установить";
            this.bSetSide.UseVisualStyleBackColor = true;
            this.bSetSide.Click += new System.EventHandler(this.bSetSide_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Притягиваемая сторона:";
            // 
            // lbSide
            // 
            this.lbSide.AutoSize = true;
            this.lbSide.ForeColor = System.Drawing.Color.Navy;
            this.lbSide.Location = new System.Drawing.Point(145, 72);
            this.lbSide.Name = "lbSide";
            this.lbSide.Size = new System.Drawing.Size(88, 13);
            this.lbSide.TabIndex = 4;
            this.lbSide.Text = "Не установлена";
            // 
            // bSelectObjects
            // 
            this.bSelectObjects.Location = new System.Drawing.Point(148, 38);
            this.bSelectObjects.Name = "bSelectObjects";
            this.bSelectObjects.Size = new System.Drawing.Size(75, 23);
            this.bSelectObjects.TabIndex = 3;
            this.bSelectObjects.Text = "Выделить";
            this.bSelectObjects.UseVisualStyleBackColor = true;
            this.bSelectObjects.Visible = false;
            this.bSelectObjects.Click += new System.EventHandler(this.bSelectObjects_Click);
            // 
            // bSetObjects
            // 
            this.bSetObjects.Location = new System.Drawing.Point(9, 38);
            this.bSetObjects.Name = "bSetObjects";
            this.bSetObjects.Size = new System.Drawing.Size(75, 23);
            this.bSetObjects.TabIndex = 2;
            this.bSetObjects.Text = "Установить";
            this.bSetObjects.UseVisualStyleBackColor = true;
            this.bSetObjects.Click += new System.EventHandler(this.bSetObjects_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Притягиваемые объекты:";
            // 
            // lbObjects
            // 
            this.lbObjects.AutoSize = true;
            this.lbObjects.ForeColor = System.Drawing.Color.Navy;
            this.lbObjects.Location = new System.Drawing.Point(145, 22);
            this.lbObjects.Name = "lbObjects";
            this.lbObjects.Size = new System.Drawing.Size(90, 13);
            this.lbObjects.TabIndex = 0;
            this.lbObjects.Text = "Не установлены";
            // 
            // gbGuide
            // 
            this.gbGuide.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbGuide.Controls.Add(this.bSelectGuide);
            this.gbGuide.Controls.Add(this.bSetGuide);
            this.gbGuide.Controls.Add(this.label3);
            this.gbGuide.Controls.Add(this.lbGuide);
            this.gbGuide.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gbGuide.Location = new System.Drawing.Point(13, 143);
            this.gbGuide.Name = "gbGuide";
            this.gbGuide.Size = new System.Drawing.Size(308, 74);
            this.gbGuide.TabIndex = 1;
            this.gbGuide.TabStop = false;
            this.gbGuide.Text = "К чему притягивать";
            // 
            // bSelectGuide
            // 
            this.bSelectGuide.Location = new System.Drawing.Point(148, 41);
            this.bSelectGuide.Name = "bSelectGuide";
            this.bSelectGuide.Size = new System.Drawing.Size(75, 23);
            this.bSelectGuide.TabIndex = 7;
            this.bSelectGuide.Text = "Выделить";
            this.bSelectGuide.UseVisualStyleBackColor = true;
            this.bSelectGuide.Visible = false;
            this.bSelectGuide.Click += new System.EventHandler(this.bSelectGuide_Click);
            // 
            // bSetGuide
            // 
            this.bSetGuide.Location = new System.Drawing.Point(9, 41);
            this.bSetGuide.Name = "bSetGuide";
            this.bSetGuide.Size = new System.Drawing.Size(75, 23);
            this.bSetGuide.TabIndex = 6;
            this.bSetGuide.Text = "Установить";
            this.bSetGuide.UseVisualStyleBackColor = true;
            this.bSetGuide.Click += new System.EventHandler(this.bSetGuide_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Направляющий отрезок:";
            // 
            // lbGuide
            // 
            this.lbGuide.AutoSize = true;
            this.lbGuide.ForeColor = System.Drawing.Color.Navy;
            this.lbGuide.Location = new System.Drawing.Point(145, 25);
            this.lbGuide.Name = "lbGuide";
            this.lbGuide.Size = new System.Drawing.Size(82, 13);
            this.lbGuide.TabIndex = 4;
            this.lbGuide.Text = "Не установлен";
            // 
            // bPulling
            // 
            this.bPulling.Location = new System.Drawing.Point(9, 50);
            this.bPulling.Name = "bPulling";
            this.bPulling.Size = new System.Drawing.Size(75, 23);
            this.bPulling.TabIndex = 2;
            this.bPulling.Text = "Притянуть";
            this.bPulling.UseVisualStyleBackColor = true;
            this.bPulling.Click += new System.EventHandler(this.bPulling_Click);
            // 
            // gbPulling
            // 
            this.gbPulling.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPulling.Controls.Add(this.tbDist);
            this.gbPulling.Controls.Add(this.label5);
            this.gbPulling.Controls.Add(this.bPulling);
            this.gbPulling.Location = new System.Drawing.Point(13, 223);
            this.gbPulling.Name = "gbPulling";
            this.gbPulling.Size = new System.Drawing.Size(308, 87);
            this.gbPulling.TabIndex = 3;
            this.gbPulling.TabStop = false;
            this.gbPulling.Text = "Притягивание";
            // 
            // tbDist
            // 
            this.tbDist.Location = new System.Drawing.Point(98, 24);
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
            // PullingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbGuide);
            this.Controls.Add(this.gbPulling);
            this.Controls.Add(this.gbObjects);
            this.Name = "PullingView";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(335, 473);
            this.gbObjects.ResumeLayout(false);
            this.gbObjects.PerformLayout();
            this.gbGuide.ResumeLayout(false);
            this.gbGuide.PerformLayout();
            this.gbPulling.ResumeLayout(false);
            this.gbPulling.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbObjects;
        private System.Windows.Forms.GroupBox gbGuide;
        private System.Windows.Forms.Button bSelectSide;
        private System.Windows.Forms.Button bSetSide;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbSide;
        private System.Windows.Forms.Button bSelectObjects;
        private System.Windows.Forms.Button bSetObjects;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbObjects;
        private System.Windows.Forms.Button bSelectGuide;
        private System.Windows.Forms.Button bSetGuide;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbGuide;
        private System.Windows.Forms.Button bPulling;
        private System.Windows.Forms.GroupBox gbPulling;
        private System.Windows.Forms.TextBox tbDist;
        private System.Windows.Forms.Label label5;
    }
}
