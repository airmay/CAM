namespace CAM.UI
{
    partial class TechProcessView
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageParams = new System.Windows.Forms.TabPage();
            this.tabPageCommands = new System.Windows.Forms.TabPage();
            this.dataGridViewCommand = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.bCreateTechOperation = new System.Windows.Forms.ToolStripButton();
            this.bRemove = new System.Windows.Forms.ToolStripButton();
            this.bMoveUpTechOperation = new System.Windows.Forms.ToolStripButton();
            this.bMoveDownTechOperation = new System.Windows.Forms.ToolStripButton();
            this.bCreateTechProcess = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bBuildProcessing = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.codeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.param1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.param2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.param3DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.param4DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.param5DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.processCommandBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageCommands.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCommand)).BeginInit();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.processCommandBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl);
            this.splitContainer1.Size = new System.Drawing.Size(312, 711);
            this.splitContainer1.SplitterDistance = 271;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.HideSelection = false;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList;
            this.treeView.LabelEdit = true;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(312, 271);
            this.treeView.TabIndex = 0;
            this.treeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_AfterLabelEdit);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageParams);
            this.tabControl.Controls.Add(this.tabPageCommands);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(312, 436);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageParams
            // 
            this.tabPageParams.Location = new System.Drawing.Point(4, 22);
            this.tabPageParams.Name = "tabPageParams";
            this.tabPageParams.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageParams.Size = new System.Drawing.Size(304, 410);
            this.tabPageParams.TabIndex = 0;
            this.tabPageParams.Text = "Параметры";
            this.tabPageParams.UseVisualStyleBackColor = true;
            // 
            // tabPageCommands
            // 
            this.tabPageCommands.Controls.Add(this.dataGridViewCommand);
            this.tabPageCommands.Location = new System.Drawing.Point(4, 22);
            this.tabPageCommands.Name = "tabPageCommands";
            this.tabPageCommands.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCommands.Size = new System.Drawing.Size(304, 410);
            this.tabPageCommands.TabIndex = 1;
            this.tabPageCommands.Text = "Программа";
            this.tabPageCommands.UseVisualStyleBackColor = true;
            // 
            // dataGridViewCommand
            // 
            this.dataGridViewCommand.AutoGenerateColumns = false;
            this.dataGridViewCommand.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            this.dataGridViewCommand.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewCommand.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCommand.ColumnHeadersVisible = false;
            this.dataGridViewCommand.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.descriptionDataGridViewTextBoxColumn,
            this.codeDataGridViewTextBoxColumn,
            this.param1DataGridViewTextBoxColumn,
            this.param2DataGridViewTextBoxColumn,
            this.param3DataGridViewTextBoxColumn,
            this.param4DataGridViewTextBoxColumn,
            this.param5DataGridViewTextBoxColumn});
            this.dataGridViewCommand.DataSource = this.processCommandBindingSource;
            this.dataGridViewCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewCommand.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridViewCommand.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewCommand.Name = "dataGridViewCommand";
            this.dataGridViewCommand.RowHeadersVisible = false;
            this.dataGridViewCommand.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewCommand.Size = new System.Drawing.Size(298, 404);
            this.dataGridViewCommand.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(312, 25);
            this.panel1.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bCreateTechOperation,
            this.bRemove,
            this.bMoveUpTechOperation,
            this.bMoveDownTechOperation,
            this.bCreateTechProcess,
            this.toolStripSeparator1,
            this.bBuildProcessing});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(312, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // bCreateTechOperation
            // 
            this.bCreateTechOperation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bCreateTechOperation.Image = global::CAM.Properties.Resources.plus;
            this.bCreateTechOperation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bCreateTechOperation.Name = "bCreateTechOperation";
            this.bCreateTechOperation.Size = new System.Drawing.Size(23, 22);
            this.bCreateTechOperation.Text = "Добавить операцию";
            this.bCreateTechOperation.Click += new System.EventHandler(this.bCreateTechOperation_Click);
            // 
            // bRemove
            // 
            this.bRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bRemove.Image = global::CAM.Properties.Resources.minus;
            this.bRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bRemove.Name = "bRemove";
            this.bRemove.Size = new System.Drawing.Size(23, 22);
            this.bRemove.Text = "Удалить";
            this.bRemove.Click += new System.EventHandler(this.bRemove_Click);
            // 
            // bMoveUpTechOperation
            // 
            this.bMoveUpTechOperation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bMoveUpTechOperation.Image = global::CAM.Properties.Resources.arrow_up;
            this.bMoveUpTechOperation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bMoveUpTechOperation.Name = "bMoveUpTechOperation";
            this.bMoveUpTechOperation.Size = new System.Drawing.Size(23, 22);
            this.bMoveUpTechOperation.Text = "Пререместить выше";
            this.bMoveUpTechOperation.Click += new System.EventHandler(this.bMoveUpTechOperation_Click);
            // 
            // bMoveDownTechOperation
            // 
            this.bMoveDownTechOperation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bMoveDownTechOperation.Image = global::CAM.Properties.Resources.arrow_down;
            this.bMoveDownTechOperation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bMoveDownTechOperation.Name = "bMoveDownTechOperation";
            this.bMoveDownTechOperation.Size = new System.Drawing.Size(23, 22);
            this.bMoveDownTechOperation.Text = "Переместить ниже";
            this.bMoveDownTechOperation.Click += new System.EventHandler(this.bMoveDownTechOperation_Click);
            // 
            // bCreateTechProcess
            // 
            this.bCreateTechProcess.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bCreateTechProcess.Image = global::CAM.Properties.Resources.drive__plus;
            this.bCreateTechProcess.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bCreateTechProcess.Name = "bCreateTechProcess";
            this.bCreateTechProcess.Size = new System.Drawing.Size(23, 22);
            this.bCreateTechProcess.Text = "Добавить техпроцесс";
            this.bCreateTechProcess.Click += new System.EventHandler(this.bCreateTechProcess_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bBuildProcessing
            // 
            this.bBuildProcessing.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bBuildProcessing.Image = global::CAM.Properties.Resources.gear;
            this.bBuildProcessing.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bBuildProcessing.Name = "bBuildProcessing";
            this.bBuildProcessing.Size = new System.Drawing.Size(23, 22);
            this.bBuildProcessing.Text = "Рассчитать обработку";
            this.bBuildProcessing.Click += new System.EventHandler(this.bBuildProcessing_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitContainer1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 25);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(312, 711);
            this.panel2.TabIndex = 3;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.Width = 5;
            // 
            // codeDataGridViewTextBoxColumn
            // 
            this.codeDataGridViewTextBoxColumn.DataPropertyName = "Code";
            this.codeDataGridViewTextBoxColumn.HeaderText = "Code";
            this.codeDataGridViewTextBoxColumn.Name = "codeDataGridViewTextBoxColumn";
            this.codeDataGridViewTextBoxColumn.Width = 5;
            // 
            // param1DataGridViewTextBoxColumn
            // 
            this.param1DataGridViewTextBoxColumn.DataPropertyName = "Param1";
            this.param1DataGridViewTextBoxColumn.HeaderText = "Param1";
            this.param1DataGridViewTextBoxColumn.Name = "param1DataGridViewTextBoxColumn";
            this.param1DataGridViewTextBoxColumn.Width = 5;
            // 
            // param2DataGridViewTextBoxColumn
            // 
            this.param2DataGridViewTextBoxColumn.DataPropertyName = "Param2";
            this.param2DataGridViewTextBoxColumn.HeaderText = "Param2";
            this.param2DataGridViewTextBoxColumn.Name = "param2DataGridViewTextBoxColumn";
            this.param2DataGridViewTextBoxColumn.Width = 5;
            // 
            // param3DataGridViewTextBoxColumn
            // 
            this.param3DataGridViewTextBoxColumn.DataPropertyName = "Param3";
            this.param3DataGridViewTextBoxColumn.HeaderText = "Param3";
            this.param3DataGridViewTextBoxColumn.Name = "param3DataGridViewTextBoxColumn";
            this.param3DataGridViewTextBoxColumn.Width = 5;
            // 
            // param4DataGridViewTextBoxColumn
            // 
            this.param4DataGridViewTextBoxColumn.DataPropertyName = "Param4";
            this.param4DataGridViewTextBoxColumn.HeaderText = "Param4";
            this.param4DataGridViewTextBoxColumn.Name = "param4DataGridViewTextBoxColumn";
            this.param4DataGridViewTextBoxColumn.Width = 5;
            // 
            // param5DataGridViewTextBoxColumn
            // 
            this.param5DataGridViewTextBoxColumn.DataPropertyName = "Param5";
            this.param5DataGridViewTextBoxColumn.HeaderText = "Param5";
            this.param5DataGridViewTextBoxColumn.Name = "param5DataGridViewTextBoxColumn";
            this.param5DataGridViewTextBoxColumn.Width = 5;
            // 
            // processCommandBindingSource
            // 
            this.processCommandBindingSource.DataSource = typeof(CAM.Domain.ProcessCommand);
            this.processCommandBindingSource.CurrentChanged += new System.EventHandler(this.processCommandBindingSource_CurrentChanged);
            // 
            // TechProcessView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "TechProcessView";
            this.Size = new System.Drawing.Size(312, 736);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPageCommands.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCommand)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.processCommandBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        public System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton bCreateTechOperation;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripButton bRemove;
        private System.Windows.Forms.ToolStripButton bMoveUpTechOperation;
        private System.Windows.Forms.ToolStripButton bMoveDownTechOperation;
        private System.Windows.Forms.ToolStripButton bCreateTechProcess;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton bBuildProcessing;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageParams;
        private System.Windows.Forms.TabPage tabPageCommands;
        private System.Windows.Forms.DataGridView dataGridViewCommand;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn codeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn param1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn param2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn param3DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn param4DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn param5DataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource processCommandBindingSource;
    }
}
