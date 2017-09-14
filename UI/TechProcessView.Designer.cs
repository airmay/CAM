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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TechProcessView));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.bCreateTechOperation = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.bRemove = new System.Windows.Forms.ToolStripButton();
            this.bMoveUpTechOperation = new System.Windows.Forms.ToolStripButton();
            this.bMoveDownTechOperation = new System.Windows.Forms.ToolStripButton();
            this.bCreateTechProcess = new System.Windows.Forms.ToolStripButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
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
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.button1);
            this.splitContainer1.Panel2.Controls.Add(this.textBox3);
            this.splitContainer1.Panel2.Controls.Add(this.textBox2);
            this.splitContainer1.Panel2.Controls.Add(this.textBox1);
            this.splitContainer1.Size = new System.Drawing.Size(312, 711);
            this.splitContainer1.SplitterDistance = 346;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.LabelEdit = true;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(312, 346);
            this.treeView.TabIndex = 0;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseDoubleClick);
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
            this.bCreateTechProcess});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(312, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // bCreateTechOperation
            // 
            this.bCreateTechOperation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bCreateTechOperation.Image = ((System.Drawing.Image)(resources.GetObject("bCreateTechOperation.Image")));
            this.bCreateTechOperation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bCreateTechOperation.Name = "bCreateTechOperation";
            this.bCreateTechOperation.Size = new System.Drawing.Size(23, 22);
            this.bCreateTechOperation.Text = "Добавить операцию";
            this.bCreateTechOperation.Click += new System.EventHandler(this.bCreateTechOperation_Click);
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
            // bRemove
            // 
            this.bRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bRemove.Image = ((System.Drawing.Image)(resources.GetObject("bRemove.Image")));
            this.bRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bRemove.Name = "bRemove";
            this.bRemove.Size = new System.Drawing.Size(23, 22);
            this.bRemove.Text = "Удалить";
            this.bRemove.Click += new System.EventHandler(this.bRemove_Click);
            // 
            // bMoveUpTechOperation
            // 
            this.bMoveUpTechOperation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bMoveUpTechOperation.Image = ((System.Drawing.Image)(resources.GetObject("bMoveUpTechOperation.Image")));
            this.bMoveUpTechOperation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bMoveUpTechOperation.Name = "bMoveUpTechOperation";
            this.bMoveUpTechOperation.Size = new System.Drawing.Size(23, 22);
            this.bMoveUpTechOperation.Text = "Пререместить выше";
            this.bMoveUpTechOperation.Click += new System.EventHandler(this.bMoveUpTechOperation_Click);
            // 
            // bMoveDownTechOperation
            // 
            this.bMoveDownTechOperation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bMoveDownTechOperation.Image = ((System.Drawing.Image)(resources.GetObject("bMoveDownTechOperation.Image")));
            this.bMoveDownTechOperation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bMoveDownTechOperation.Name = "bMoveDownTechOperation";
            this.bMoveDownTechOperation.Size = new System.Drawing.Size(23, 22);
            this.bMoveDownTechOperation.Text = "Переместить ниже";
            this.bMoveDownTechOperation.Click += new System.EventHandler(this.bMoveDownTechOperation_Click);
            // 
            // bCreateTechProcess
            // 
            this.bCreateTechProcess.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bCreateTechProcess.Image = ((System.Drawing.Image)(resources.GetObject("bCreateTechProcess.Image")));
            this.bCreateTechProcess.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bCreateTechProcess.Name = "bCreateTechProcess";
            this.bCreateTechProcess.Size = new System.Drawing.Size(23, 22);
            this.bCreateTechProcess.Text = "Добавить изделие";
            this.bCreateTechProcess.Click += new System.EventHandler(this.bCreateTechProcess_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(65, 39);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 0;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(65, 120);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 1;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(65, 81);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 20);
            this.textBox3.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(99, 167);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
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
            this.splitContainer1.Panel2.PerformLayout();
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
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
    }
}
