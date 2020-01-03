namespace CAM
{
	partial class BorderProcessingAreaView
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
            this.cbExactlyEnd = new System.Windows.Forms.CheckBox();
            this.cbExactlyBegin = new System.Windows.Forms.CheckBox();
            this.cbAutoExactlyEnd = new System.Windows.Forms.CheckBox();
            this.cbAutoExactlyBegin = new System.Windows.Forms.CheckBox();
            this.borderProcessingAreaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.borderProcessingAreaBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // cbExactlyEnd
            // 
            this.cbExactlyEnd.AutoSize = true;
            this.cbExactlyEnd.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.borderProcessingAreaBindingSource, "IsExactlyEnd", true));
            this.cbExactlyEnd.Location = new System.Drawing.Point(3, 26);
            this.cbExactlyEnd.Name = "cbExactlyEnd";
            this.cbExactlyEnd.Size = new System.Drawing.Size(88, 17);
            this.cbExactlyEnd.TabIndex = 6;
            this.cbExactlyEnd.Text = "Конец точно";
            this.cbExactlyEnd.UseVisualStyleBackColor = true;
            this.cbExactlyEnd.Click += new System.EventHandler(this.cbExactlyEnd_Click);
            // 
            // cbExactlyBegin
            // 
            this.cbExactlyBegin.AutoSize = true;
            this.cbExactlyBegin.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.borderProcessingAreaBindingSource, "IsExactlyBegin", true));
            this.cbExactlyBegin.Location = new System.Drawing.Point(3, 3);
            this.cbExactlyBegin.Name = "cbExactlyBegin";
            this.cbExactlyBegin.Size = new System.Drawing.Size(94, 17);
            this.cbExactlyBegin.TabIndex = 4;
            this.cbExactlyBegin.Text = "Начало точно";
            this.cbExactlyBegin.UseVisualStyleBackColor = true;
            this.cbExactlyBegin.Click += new System.EventHandler(this.cbExactlyBegin_Click);
            // 
            // cbAutoExactlyEnd
            // 
            this.cbAutoExactlyEnd.AutoSize = true;
            this.cbAutoExactlyEnd.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.borderProcessingAreaBindingSource, "IsAutoExactlyEnd", true));
            this.cbAutoExactlyEnd.Location = new System.Drawing.Point(103, 26);
            this.cbAutoExactlyEnd.Name = "cbAutoExactlyEnd";
            this.cbAutoExactlyEnd.Size = new System.Drawing.Size(50, 17);
            this.cbAutoExactlyEnd.TabIndex = 7;
            this.cbAutoExactlyEnd.Text = "Авто";
            this.cbAutoExactlyEnd.UseVisualStyleBackColor = true;
            // 
            // cbAutoExactlyBegin
            // 
            this.cbAutoExactlyBegin.AutoSize = true;
            this.cbAutoExactlyBegin.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.borderProcessingAreaBindingSource, "IsAutoExactlyBegin", true));
            this.cbAutoExactlyBegin.Location = new System.Drawing.Point(103, 3);
            this.cbAutoExactlyBegin.Name = "cbAutoExactlyBegin";
            this.cbAutoExactlyBegin.Size = new System.Drawing.Size(50, 17);
            this.cbAutoExactlyBegin.TabIndex = 5;
            this.cbAutoExactlyBegin.Text = "Авто";
            this.cbAutoExactlyBegin.UseVisualStyleBackColor = true;
            // 
            // borderProcessingAreaBindingSource
            // 
            this.borderProcessingAreaBindingSource.DataSource = typeof(BorderProcessingArea);
            // 
            // BorderProcessingAreaView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbAutoExactlyEnd);
            this.Controls.Add(this.cbAutoExactlyBegin);
            this.Controls.Add(this.cbExactlyEnd);
            this.Controls.Add(this.cbExactlyBegin);
            this.Name = "BorderProcessingAreaView";
            this.Size = new System.Drawing.Size(154, 46);
            ((System.ComponentModel.ISupportInitialize)(this.borderProcessingAreaBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox cbExactlyEnd;
		private System.Windows.Forms.CheckBox cbExactlyBegin;
		private System.Windows.Forms.CheckBox cbAutoExactlyEnd;
		private System.Windows.Forms.CheckBox cbAutoExactlyBegin;
		private System.Windows.Forms.BindingSource borderProcessingAreaBindingSource;
    }
}
