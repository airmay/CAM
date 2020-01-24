namespace CAM.TechOperation.Tactile
{
    partial class TactileParamsView
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
            this.tactileDefaultParamsView1 = new CAM.TechOperation.Tactile.TactileDefaultParamsView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bCalculate = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.posDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CuttingType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.passListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tactileParamsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.passListBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tactileParamsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tactileDefaultParamsView1
            // 
            this.tactileDefaultParamsView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tactileDefaultParamsView1.Location = new System.Drawing.Point(0, 0);
            this.tactileDefaultParamsView1.Name = "tactileDefaultParamsView1";
            this.tactileDefaultParamsView1.Size = new System.Drawing.Size(286, 356);
            this.tactileDefaultParamsView1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.bCalculate);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 356);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(286, 32);
            this.panel1.TabIndex = 1;
            // 
            // bCalculate
            // 
            this.bCalculate.Location = new System.Drawing.Point(3, 6);
            this.bCalculate.Name = "bCalculate";
            this.bCalculate.Size = new System.Drawing.Size(75, 23);
            this.bCalculate.TabIndex = 0;
            this.bCalculate.Text = "Расчитать";
            this.bCalculate.UseVisualStyleBackColor = true;
            this.bCalculate.Click += new System.EventHandler(this.bCalculate_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.posDataGridViewTextBoxColumn,
            this.CuttingType});
            this.dataGridView1.DataSource = this.passListBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 388);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(286, 138);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            // 
            // posDataGridViewTextBoxColumn
            // 
            this.posDataGridViewTextBoxColumn.DataPropertyName = "Pos";
            this.posDataGridViewTextBoxColumn.HeaderText = "Позиция";
            this.posDataGridViewTextBoxColumn.Name = "posDataGridViewTextBoxColumn";
            // 
            // CuttingType
            // 
            this.CuttingType.DataPropertyName = "CuttingType";
            this.CuttingType.DisplayStyleForCurrentCellOnly = true;
            this.CuttingType.HeaderText = "Тип";
            this.CuttingType.Items.AddRange(new object[] {
            "Гребенка",
            "Чистка"});
            this.CuttingType.Name = "CuttingType";
            // 
            // passListBindingSource
            // 
            this.passListBindingSource.AllowNew = true;
            this.passListBindingSource.DataMember = "PassList";
            this.passListBindingSource.DataSource = this.tactileParamsBindingSource;
            // 
            // tactileParamsBindingSource
            // 
            this.tactileParamsBindingSource.DataSource = typeof(CAM.TechOperation.Tactile.TactileParams);
            // 
            // TactileParamsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tactileDefaultParamsView1);
            this.Name = "TactileParamsView";
            this.Size = new System.Drawing.Size(286, 526);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.passListBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tactileParamsBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TactileDefaultParamsView tactileDefaultParamsView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button bCalculate;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource passListBindingSource;
        private System.Windows.Forms.BindingSource tactileParamsBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn posDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn CuttingType;
    }
}
