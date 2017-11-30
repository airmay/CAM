namespace CAM.UI
{
    partial class SawingParamsView
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
            this.gbExactlyEnds = new System.Windows.Forms.GroupBox();
            this.cbExactlyEnd = new System.Windows.Forms.CheckBox();
            this.sawingParamsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cbExactlyBegin = new System.Windows.Forms.CheckBox();
            this.gvSawingModes = new System.Windows.Forms.DataGridView();
            this.depthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.depthStepDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.feedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbExactlyEnds.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sawingParamsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSawingModes)).BeginInit();
            this.SuspendLayout();
            // 
            // gbExactlyEnds
            // 
            this.gbExactlyEnds.Controls.Add(this.cbExactlyEnd);
            this.gbExactlyEnds.Controls.Add(this.cbExactlyBegin);
            this.gbExactlyEnds.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbExactlyEnds.Location = new System.Drawing.Point(0, 0);
            this.gbExactlyEnds.Name = "gbExactlyEnds";
            this.gbExactlyEnds.Size = new System.Drawing.Size(245, 45);
            this.gbExactlyEnds.TabIndex = 0;
            this.gbExactlyEnds.TabStop = false;
            this.gbExactlyEnds.Text = "Концы";
            // 
            // cbExactlyEnd
            // 
            this.cbExactlyEnd.AutoSize = true;
            this.cbExactlyEnd.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.sawingParamsBindingSource, "IsExactlyEnd", true));
            this.cbExactlyEnd.Location = new System.Drawing.Point(124, 19);
            this.cbExactlyEnd.Name = "cbExactlyEnd";
            this.cbExactlyEnd.Size = new System.Drawing.Size(88, 17);
            this.cbExactlyEnd.TabIndex = 3;
            this.cbExactlyEnd.Text = "Конец точно";
            this.cbExactlyEnd.UseVisualStyleBackColor = true;
            // 
            // sawingParamsBindingSource
            // 
            this.sawingParamsBindingSource.DataSource = typeof(CAM.Domain.SawingTechOperationParams);
            // 
            // cbExactlyBegin
            // 
            this.cbExactlyBegin.AutoSize = true;
            this.cbExactlyBegin.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.sawingParamsBindingSource, "IsExactlyBegin", true));
            this.cbExactlyBegin.Location = new System.Drawing.Point(18, 19);
            this.cbExactlyBegin.Name = "cbExactlyBegin";
            this.cbExactlyBegin.Size = new System.Drawing.Size(94, 17);
            this.cbExactlyBegin.TabIndex = 2;
            this.cbExactlyBegin.Text = "Начало точно";
            this.cbExactlyBegin.UseVisualStyleBackColor = true;
            // 
            // gvSawingModes
            // 
            this.gvSawingModes.AutoGenerateColumns = false;
            this.gvSawingModes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.gvSawingModes.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gvSawingModes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gvSawingModes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gvSawingModes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.depthDataGridViewTextBoxColumn,
            this.depthStepDataGridViewTextBoxColumn,
            this.feedDataGridViewTextBoxColumn});
            this.gvSawingModes.DataMember = "Modes";
            this.gvSawingModes.DataSource = this.sawingParamsBindingSource;
            this.gvSawingModes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvSawingModes.Location = new System.Drawing.Point(0, 45);
            this.gvSawingModes.MultiSelect = false;
            this.gvSawingModes.Name = "gvSawingModes";
            this.gvSawingModes.Size = new System.Drawing.Size(245, 366);
            this.gvSawingModes.TabIndex = 1;
            this.gvSawingModes.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.gvSawingModes_DataError);
            // 
            // depthDataGridViewTextBoxColumn
            // 
            this.depthDataGridViewTextBoxColumn.DataPropertyName = "Depth";
            this.depthDataGridViewTextBoxColumn.HeaderText = "Глубина";
            this.depthDataGridViewTextBoxColumn.Name = "depthDataGridViewTextBoxColumn";
            this.depthDataGridViewTextBoxColumn.Width = 73;
            // 
            // depthStepDataGridViewTextBoxColumn
            // 
            this.depthStepDataGridViewTextBoxColumn.DataPropertyName = "DepthStep";
            this.depthStepDataGridViewTextBoxColumn.HeaderText = "Шаг";
            this.depthStepDataGridViewTextBoxColumn.Name = "depthStepDataGridViewTextBoxColumn";
            this.depthStepDataGridViewTextBoxColumn.ToolTipText = "Шаг по глубине";
            this.depthStepDataGridViewTextBoxColumn.Width = 52;
            // 
            // feedDataGridViewTextBoxColumn
            // 
            this.feedDataGridViewTextBoxColumn.DataPropertyName = "Feed";
            this.feedDataGridViewTextBoxColumn.HeaderText = "Подача";
            this.feedDataGridViewTextBoxColumn.Name = "feedDataGridViewTextBoxColumn";
            this.feedDataGridViewTextBoxColumn.Width = 69;
            // 
            // SawingParamsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gvSawingModes);
            this.Controls.Add(this.gbExactlyEnds);
            this.Name = "SawingParamsView";
            this.Size = new System.Drawing.Size(245, 411);
            this.gbExactlyEnds.ResumeLayout(false);
            this.gbExactlyEnds.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sawingParamsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSawingModes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbExactlyEnds;
        private System.Windows.Forms.CheckBox cbExactlyEnd;
        private System.Windows.Forms.CheckBox cbExactlyBegin;
        private System.Windows.Forms.DataGridView gvSawingModes;
        private System.Windows.Forms.DataGridViewTextBoxColumn depthDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn depthStepDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn feedDataGridViewTextBoxColumn;
        public System.Windows.Forms.BindingSource sawingParamsBindingSource;
    }
}
