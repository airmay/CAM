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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
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
            this.splitContainer1.Size = new System.Drawing.Size(312, 711);
            this.splitContainer1.SplitterDistance = 346;
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
            this.treeView.Size = new System.Drawing.Size(312, 346);
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
            this.bCreateTechProcess.Text = "Добавить изделие";
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
            // TechProcessView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "TechProcessView";
            this.Size = new System.Drawing.Size(312, 736);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
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
    }
}
