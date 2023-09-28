namespace CAM.TechProcesses.Sawing
{
    partial class SawingModesView
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
            this.gvSawingModes = new System.Windows.Forms.DataGridView();
            this.depthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.depthStepDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.feedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sawingModesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gvSawingModes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sawingModesBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // gvSawingModes
            // 
            this.gvSawingModes.AutoGenerateColumns = false;
            this.gvSawingModes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.gvSawingModes.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gvSawingModes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gvSawingModes.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gvSawingModes.ColumnHeadersHeight = 58;
            this.gvSawingModes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.depthDataGridViewTextBoxColumn,
            this.depthStepDataGridViewTextBoxColumn,
            this.feedDataGridViewTextBoxColumn});
            this.gvSawingModes.DataSource = this.sawingModesBindingSource;
            this.gvSawingModes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvSawingModes.Location = new System.Drawing.Point(0, 0);
            this.gvSawingModes.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.gvSawingModes.MultiSelect = false;
            this.gvSawingModes.Name = "gvSawingModes";
            this.gvSawingModes.RowHeadersVisible = false;
            this.gvSawingModes.RowHeadersWidth = 102;
            this.gvSawingModes.Size = new System.Drawing.Size(704, 672);
            this.gvSawingModes.TabIndex = 1;
            this.gvSawingModes.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.gvSawingModes_DataError);
            // 
            // depthDataGridViewTextBoxColumn
            // 
            this.depthDataGridViewTextBoxColumn.DataPropertyName = "Depth";
            this.depthDataGridViewTextBoxColumn.HeaderText = "Глубина";
            this.depthDataGridViewTextBoxColumn.MinimumWidth = 12;
            this.depthDataGridViewTextBoxColumn.Name = "depthDataGridViewTextBoxColumn";
            this.depthDataGridViewTextBoxColumn.Width = 178;
            // 
            // depthStepDataGridViewTextBoxColumn
            // 
            this.depthStepDataGridViewTextBoxColumn.DataPropertyName = "DepthStep";
            this.depthStepDataGridViewTextBoxColumn.HeaderText = "Шаг";
            this.depthStepDataGridViewTextBoxColumn.MinimumWidth = 12;
            this.depthStepDataGridViewTextBoxColumn.Name = "depthStepDataGridViewTextBoxColumn";
            this.depthStepDataGridViewTextBoxColumn.ToolTipText = "Шаг по глубине";
            this.depthStepDataGridViewTextBoxColumn.Width = 119;
            // 
            // feedDataGridViewTextBoxColumn
            // 
            this.feedDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.feedDataGridViewTextBoxColumn.DataPropertyName = "Feed";
            this.feedDataGridViewTextBoxColumn.HeaderText = "Подача";
            this.feedDataGridViewTextBoxColumn.MinimumWidth = 12;
            this.feedDataGridViewTextBoxColumn.Name = "feedDataGridViewTextBoxColumn";
            // 
            // sawingModesBindingSource
            // 
            this.sawingModesBindingSource.DataSource = typeof(CAM.TechProcesses.Sawing.SawingMode);
            // 
            // SawingModesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gvSawingModes);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "SawingModesView";
            this.Size = new System.Drawing.Size(704, 672);
            ((System.ComponentModel.ISupportInitialize)(this.gvSawingModes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sawingModesBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gvSawingModes;
        public System.Windows.Forms.BindingSource sawingModesBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn depthDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn depthStepDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn feedDataGridViewTextBoxColumn;
    }
}
