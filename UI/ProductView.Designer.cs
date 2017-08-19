namespace CAM.UI
{
    partial class ProductView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductView));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Depth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DepthStep = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Speed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.productBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.segmentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.pProduct = new System.Windows.Forms.Panel();
            this.bAddProdict = new System.Windows.Forms.Button();
            this.cbProduct = new System.Windows.Forms.ComboBox();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.segmentBindingSource)).BeginInit();
            this.pProduct.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(325, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoGenerateColumns = false;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.Depth,
            this.DepthStep,
            this.Speed});
            this.dataGridView.DataMember = "Segments";
            this.dataGridView.DataSource = this.productBindingSource;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 48);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(325, 395);
            this.dataGridView.TabIndex = 2;
            this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Наименование";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.Width = 108;
            // 
            // Depth
            // 
            this.Depth.DataPropertyName = "Depth";
            this.Depth.HeaderText = "Глубина";
            this.Depth.Name = "Depth";
            this.Depth.Width = 73;
            // 
            // DepthStep
            // 
            this.DepthStep.DataPropertyName = "DepthStep";
            this.DepthStep.HeaderText = "Шаг по Z";
            this.DepthStep.Name = "DepthStep";
            this.DepthStep.Width = 77;
            // 
            // Speed
            // 
            this.Speed.DataPropertyName = "Speed";
            this.Speed.HeaderText = "Подача";
            this.Speed.Name = "Speed";
            this.Speed.Width = 69;
            // 
            // productBindingSource
            // 
            this.productBindingSource.DataSource = typeof(CAM.Domain.Product);
            // 
            // segmentBindingSource
            // 
            this.segmentBindingSource.DataSource = typeof(CAM.Domain.Segment);
            // 
            // pProduct
            // 
            this.pProduct.Controls.Add(this.bAddProdict);
            this.pProduct.Controls.Add(this.cbProduct);
            this.pProduct.Dock = System.Windows.Forms.DockStyle.Top;
            this.pProduct.Location = new System.Drawing.Point(0, 25);
            this.pProduct.Name = "pProduct";
            this.pProduct.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.pProduct.Size = new System.Drawing.Size(325, 23);
            this.pProduct.TabIndex = 1;
            // 
            // bAddProdict
            // 
            this.bAddProdict.Dock = System.Windows.Forms.DockStyle.Right;
            this.bAddProdict.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bAddProdict.Location = new System.Drawing.Point(275, 0);
            this.bAddProdict.Name = "bAddProdict";
            this.bAddProdict.Size = new System.Drawing.Size(50, 22);
            this.bAddProdict.TabIndex = 1;
            this.bAddProdict.Text = "+";
            this.bAddProdict.UseVisualStyleBackColor = true;
            this.bAddProdict.Click += new System.EventHandler(this.bAddProdict_Click);
            // 
            // cbProduct
            // 
            this.cbProduct.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbProduct.DataSource = this.productBindingSource;
            this.cbProduct.DisplayMember = "Name";
            this.cbProduct.FormattingEnabled = true;
            this.cbProduct.Location = new System.Drawing.Point(0, 0);
            this.cbProduct.Name = "cbProduct";
            this.cbProduct.Size = new System.Drawing.Size(275, 21);
            this.cbProduct.TabIndex = 0;
            this.cbProduct.Validating += new System.ComponentModel.CancelEventHandler(this.cbProduct_Validating);
            this.cbProduct.Validated += new System.EventHandler(this.cbProduct_Validated);
            // 
            // ProductView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.pProduct);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ProductView";
            this.Size = new System.Drawing.Size(325, 443);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.segmentBindingSource)).EndInit();
            this.pProduct.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Panel pProduct;
        private System.Windows.Forms.Button bAddProdict;
        private System.Windows.Forms.ComboBox cbProduct;
        private System.Windows.Forms.BindingSource segmentBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Depth;
        private System.Windows.Forms.DataGridViewTextBoxColumn DepthStep;
        private System.Windows.Forms.DataGridViewTextBoxColumn Speed;
        private System.Windows.Forms.BindingSource productBindingSource;
    }
}
