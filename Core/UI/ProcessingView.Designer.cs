﻿namespace CAM
{
    partial class ProcessingView
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
            this.U = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.V = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.A = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.processCommandBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.bCreateTechOperation = new System.Windows.Forms.ToolStripDropDownButton();
            this.bCreateGeneralOperation = new System.Windows.Forms.ToolStripButton();
            this.bRemove = new System.Windows.Forms.ToolStripButton();
            this.bMoveUpTechOperation = new System.Windows.Forms.ToolStripButton();
            this.bMoveDownTechOperation = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bBuildProcessing = new System.Windows.Forms.ToolStripSplitButton();
            this.bPartialProcessing = new System.Windows.Forms.ToolStripMenuItem();
            this.bVisibility = new System.Windows.Forms.ToolStripButton();
            this.bPlay = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.bSendProgramm = new System.Windows.Forms.ToolStripButton();
            this.bClose = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageCommands.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCommand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.processCommandBindingSource)).BeginInit();
            this.panel1.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.splitContainer1.Size = new System.Drawing.Size(336, 673);
            this.splitContainer1.SplitterDistance = 152;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.CheckBoxes = true;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.HideSelection = false;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(336, 152);
            this.treeView.TabIndex = 0;
            this.treeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_AfterLabelEdit);
            this.treeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterCheck);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseDoubleClick);
            this.treeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView_KeyDown);
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
            this.tabControl.Size = new System.Drawing.Size(336, 517);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageParams
            // 
            this.tabPageParams.AutoScroll = true;
            this.tabPageParams.Location = new System.Drawing.Point(4, 22);
            this.tabPageParams.Name = "tabPageParams";
            this.tabPageParams.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPageParams.Size = new System.Drawing.Size(328, 491);
            this.tabPageParams.TabIndex = 0;
            this.tabPageParams.Text = "Параметры";
            this.tabPageParams.UseVisualStyleBackColor = true;
            // 
            // tabPageCommands
            // 
            this.tabPageCommands.Controls.Add(this.dataGridViewCommand);
            this.tabPageCommands.Location = new System.Drawing.Point(4, 22);
            this.tabPageCommands.Name = "tabPageCommands";
            this.tabPageCommands.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPageCommands.Size = new System.Drawing.Size(324, 493);
            this.tabPageCommands.TabIndex = 1;
            this.tabPageCommands.Text = "Программа";
            this.tabPageCommands.UseVisualStyleBackColor = true;
            // 
            // dataGridViewCommand
            // 
            this.dataGridViewCommand.AllowUserToAddRows = false;
            this.dataGridViewCommand.AllowUserToResizeRows = false;
            this.dataGridViewCommand.AutoGenerateColumns = false;
            this.dataGridViewCommand.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            this.dataGridViewCommand.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewCommand.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridViewCommand.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCommand.ColumnHeadersVisible = false;
            this.dataGridViewCommand.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.U,
            this.V,
            this.A});
            this.dataGridViewCommand.DataSource = this.processCommandBindingSource;
            this.dataGridViewCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewCommand.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewCommand.MultiSelect = false;
            this.dataGridViewCommand.Name = "dataGridViewCommand";
            this.dataGridViewCommand.RowHeadersVisible = false;
            this.dataGridViewCommand.RowHeadersWidth = 102;
            this.dataGridViewCommand.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewCommand.Size = new System.Drawing.Size(318, 487);
            this.dataGridViewCommand.TabIndex = 0;
            // 
            // U
            // 
            this.U.DataPropertyName = "U";
            this.U.HeaderText = "U";
            this.U.MinimumWidth = 12;
            this.U.Name = "U";
            this.U.Width = 12;
            // 
            // V
            // 
            this.V.DataPropertyName = "V";
            this.V.HeaderText = "V";
            this.V.MinimumWidth = 12;
            this.V.Name = "V";
            this.V.Width = 12;
            // 
            // A
            // 
            this.A.DataPropertyName = "A";
            this.A.HeaderText = "A";
            this.A.MinimumWidth = 12;
            this.A.Name = "A";
            this.A.Width = 12;
            // 
            // processCommandBindingSource
            // 
            this.processCommandBindingSource.CurrentChanged += new System.EventHandler(this.processCommandBindingSource_CurrentChanged);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.toolStrip);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(336, 25);
            this.panel1.TabIndex = 2;
            // 
            // toolStrip
            // 
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bCreateTechOperation,
            this.bCreateGeneralOperation,
            this.bRemove,
            this.bMoveUpTechOperation,
            this.bMoveDownTechOperation,
            this.toolStripSeparator1,
            this.bBuildProcessing,
            this.bVisibility,
            this.bPlay,
            this.toolStripSeparator2,
            this.bSendProgramm,
            this.bClose});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip.Size = new System.Drawing.Size(336, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // bCreateTechOperation
            // 
            this.bCreateTechOperation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bCreateTechOperation.Image = global::CAM.Properties.Resources.plus;
            this.bCreateTechOperation.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.bCreateTechOperation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bCreateTechOperation.Name = "bCreateTechOperation";
            this.bCreateTechOperation.Size = new System.Drawing.Size(29, 22);
            // 
            // bCreateGeneralOperation
            // 
            this.bCreateGeneralOperation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bCreateGeneralOperation.Image = global::CAM.Properties.Resources.folder__plus;
            this.bCreateGeneralOperation.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.bCreateGeneralOperation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bCreateGeneralOperation.Name = "bCreateGeneralOperation";
            this.bCreateGeneralOperation.Size = new System.Drawing.Size(23, 22);
            this.bCreateGeneralOperation.Text = "bCreateGeneralOperation";
            this.bCreateGeneralOperation.Click += new System.EventHandler(this.bCreateGeneralOperation_Click);
            // 
            // bRemove
            // 
            this.bRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bRemove.Image = global::CAM.Properties.Resources.cross;
            this.bRemove.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
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
            this.bMoveUpTechOperation.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
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
            this.bMoveDownTechOperation.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.bMoveDownTechOperation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bMoveDownTechOperation.Name = "bMoveDownTechOperation";
            this.bMoveDownTechOperation.Size = new System.Drawing.Size(23, 22);
            this.bMoveDownTechOperation.Text = "Переместить ниже";
            this.bMoveDownTechOperation.Click += new System.EventHandler(this.bMoveDownTechOperation_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bBuildProcessing
            // 
            this.bBuildProcessing.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bBuildProcessing.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bPartialProcessing});
            this.bBuildProcessing.Image = global::CAM.Properties.Resources.gear;
            this.bBuildProcessing.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.bBuildProcessing.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bBuildProcessing.Name = "bBuildProcessing";
            this.bBuildProcessing.Size = new System.Drawing.Size(32, 22);
            this.bBuildProcessing.Text = "Рассчитать обработку";
            this.bBuildProcessing.ButtonClick += new System.EventHandler(this.bBuildProcessing_ButtonClick);
            // 
            // bPartialProcessing
            // 
            this.bPartialProcessing.Name = "bPartialProcessing";
            this.bPartialProcessing.Size = new System.Drawing.Size(193, 22);
            this.bPartialProcessing.Text = "Частичная обработка";
            this.bPartialProcessing.ToolTipText = "Формирование программы обработки с текущей команды";
            this.bPartialProcessing.Click += new System.EventHandler(this.bBuildProcessing_ButtonClick);
            // 
            // bVisibility
            // 
            this.bVisibility.CheckOnClick = true;
            this.bVisibility.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bVisibility.Image = global::CAM.Properties.Resources.eraser;
            this.bVisibility.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.bVisibility.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bVisibility.Name = "bVisibility";
            this.bVisibility.Size = new System.Drawing.Size(23, 22);
            this.bVisibility.Text = "Удалить доп. объекты";
            this.bVisibility.Click += new System.EventHandler(this.bVisibility_Click);
            // 
            // bPlay
            // 
            this.bPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bPlay.Image = global::CAM.Properties.Resources.icons8_cinema_16;
            this.bPlay.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.bPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bPlay.Name = "bPlay";
            this.bPlay.Size = new System.Drawing.Size(23, 22);
            this.bPlay.Text = "Проиграть обработку";
            this.bPlay.ToolTipText = "Проигрывавание обработки";
            this.bPlay.Click += new System.EventHandler(this.bPlay_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // bSendProgramm
            // 
            this.bSendProgramm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bSendProgramm.Image = global::CAM.Properties.Resources.disk__arrow;
            this.bSendProgramm.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.bSendProgramm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bSendProgramm.Name = "bSendProgramm";
            this.bSendProgramm.Size = new System.Drawing.Size(23, 22);
            this.bSendProgramm.Text = "Записать файл с программой";
            this.bSendProgramm.Click += new System.EventHandler(this.bSend_Click);
            // 
            // bClose
            // 
            this.bClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bClose.Image = global::CAM.Properties.Resources.cross;
            this.bClose.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.bClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(23, 22);
            this.bClose.Text = "Закрыть Автокад";
            this.bClose.Visible = false;
            this.bClose.Click += new System.EventHandler(this.bClose_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitContainer1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 25);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(336, 673);
            this.panel2.TabIndex = 3;
            // 
            // ProcessingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ProcessingView";
            this.Size = new System.Drawing.Size(336, 698);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPageCommands.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCommand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.processCommandBindingSource)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        public System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripButton bRemove;
        private System.Windows.Forms.ToolStripButton bMoveUpTechOperation;
        private System.Windows.Forms.ToolStripButton bMoveDownTechOperation;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageParams;
        private System.Windows.Forms.TabPage tabPageCommands;
        private System.Windows.Forms.DataGridView dataGridViewCommand;
        private System.Windows.Forms.BindingSource processCommandBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn codeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn param1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn param2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn param3DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn param4DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn param5DataGridViewTextBoxColumn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton bSendProgramm;
        private System.Windows.Forms.ToolStripButton bClose;
        private System.Windows.Forms.ToolStripButton bPlay;
        private System.Windows.Forms.ToolStripDropDownButton bCreateTechOperation;
        private System.Windows.Forms.ToolStripButton bVisibility;
        private System.Windows.Forms.ToolStripSplitButton bBuildProcessing;
        private System.Windows.Forms.ToolStripMenuItem bPartialProcessing;
        private System.Windows.Forms.DataGridViewTextBoxColumn numberDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn textDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn U;
        private System.Windows.Forms.DataGridViewTextBoxColumn V;
        private System.Windows.Forms.DataGridViewTextBoxColumn A;
        private System.Windows.Forms.ToolStripButton bCreateGeneralOperation;
    }
}
