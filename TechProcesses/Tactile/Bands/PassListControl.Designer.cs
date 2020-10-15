namespace CAM.TechProcesses.Tactile
{
    partial class PassListControl
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.bDown = new System.Windows.Forms.Button();
            this.bUp = new System.Windows.Forms.Button();
            this.bCalculate = new System.Windows.Forms.Button();
            this.gvPassList = new System.Windows.Forms.DataGridView();
            this.posDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cuttingTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvPassList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.bDown);
            this.panel1.Controls.Add(this.bUp);
            this.panel1.Controls.Add(this.bCalculate);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(225, 30);
            this.panel1.TabIndex = 0;
            // 
            // bDown
            // 
            this.bDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bDown.Location = new System.Drawing.Point(169, 3);
            this.bDown.Name = "bDown";
            this.bDown.Size = new System.Drawing.Size(53, 23);
            this.bDown.TabIndex = 65;
            this.bDown.Text = "Вниз";
            this.bDown.UseVisualStyleBackColor = true;
            this.bDown.Click += new System.EventHandler(this.bDown_Click);
            // 
            // bUp
            // 
            this.bUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bUp.Location = new System.Drawing.Point(110, 3);
            this.bUp.Name = "bUp";
            this.bUp.Size = new System.Drawing.Size(53, 23);
            this.bUp.TabIndex = 64;
            this.bUp.Text = "Вверх";
            this.bUp.UseVisualStyleBackColor = true;
            this.bUp.Click += new System.EventHandler(this.bUp_Click);
            // 
            // bCalculate
            // 
            this.bCalculate.Location = new System.Drawing.Point(3, 3);
            this.bCalculate.Name = "bCalculate";
            this.bCalculate.Size = new System.Drawing.Size(75, 23);
            this.bCalculate.TabIndex = 63;
            this.bCalculate.Text = "Расчитать";
            this.bCalculate.UseVisualStyleBackColor = true;
            this.bCalculate.Click += new System.EventHandler(this.bCalculate_Click);
            // 
            // gvPassList
            // 
            this.gvPassList.AllowUserToResizeRows = false;
            this.gvPassList.AutoGenerateColumns = false;
            this.gvPassList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gvPassList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gvPassList.ColumnHeadersVisible = false;
            this.gvPassList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.posDataGridViewTextBoxColumn,
            this.cuttingTypeDataGridViewTextBoxColumn});
            this.gvPassList.DataSource = this.bindingSource;
            this.gvPassList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvPassList.Location = new System.Drawing.Point(0, 30);
            this.gvPassList.MultiSelect = false;
            this.gvPassList.Name = "gvPassList";
            this.gvPassList.Size = new System.Drawing.Size(225, 250);
            this.gvPassList.TabIndex = 61;
            // 
            // posDataGridViewTextBoxColumn
            // 
            this.posDataGridViewTextBoxColumn.DataPropertyName = "Pos";
            this.posDataGridViewTextBoxColumn.HeaderText = "Pos";
            this.posDataGridViewTextBoxColumn.Name = "posDataGridViewTextBoxColumn";
            // 
            // cuttingTypeDataGridViewTextBoxColumn
            // 
            this.cuttingTypeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cuttingTypeDataGridViewTextBoxColumn.DataPropertyName = "CuttingType";
            this.cuttingTypeDataGridViewTextBoxColumn.DisplayStyleForCurrentCellOnly = true;
            this.cuttingTypeDataGridViewTextBoxColumn.HeaderText = "CuttingType";
            this.cuttingTypeDataGridViewTextBoxColumn.Items.AddRange(new object[] {
            "Гребенка",
            "Чистка"});
            this.cuttingTypeDataGridViewTextBoxColumn.Name = "cuttingTypeDataGridViewTextBoxColumn";
            this.cuttingTypeDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cuttingTypeDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // bindingSource
            // 
            this.bindingSource.DataMember = "PassList";
            this.bindingSource.DataSource = typeof(CAM.Tactile.BandsTechOperation);
            // 
            // PassListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gvPassList);
            this.Controls.Add(this.panel1);
            this.Name = "PassListControl";
            this.Size = new System.Drawing.Size(225, 280);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvPassList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button bDown;
        private System.Windows.Forms.Button bUp;
        private System.Windows.Forms.Button bCalculate;
        private System.Windows.Forms.DataGridView gvPassList;
        private System.Windows.Forms.BindingSource bindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn posDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn cuttingTypeDataGridViewTextBoxColumn;
    }
}
