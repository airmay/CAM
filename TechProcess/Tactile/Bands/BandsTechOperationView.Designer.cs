namespace CAM.Tactile
{
    partial class BandsTechOperationView
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
            this.tbFeed = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbProcessingAngle = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbDepth = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbBandWidth = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbBandStart = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbBandSpacing = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bCalculate = new System.Windows.Forms.Button();
            this.gvPassList = new System.Windows.Forms.DataGridView();
            this.passListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tbMaxCrestWidth = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbFeedFinishing = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.bUp = new System.Windows.Forms.Button();
            this.bDown = new System.Windows.Forms.Button();
            this.bandsTechOperationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.posDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CuttingType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gvPassList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.passListBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bandsTechOperationBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tbFeed
            // 
            this.tbFeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFeed.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bandsTechOperationBindingSource, "Feed", true));
            this.tbFeed.Location = new System.Drawing.Point(102, 26);
            this.tbFeed.Name = "tbFeed";
            this.tbFeed.Size = new System.Drawing.Size(152, 20);
            this.tbFeed.TabIndex = 33;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 29);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(94, 13);
            this.label11.TabIndex = 43;
            this.label11.Text = "Подача гребенка";
            // 
            // tbProcessingAngle
            // 
            this.tbProcessingAngle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProcessingAngle.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bandsTechOperationBindingSource, "ProcessingAngle", true));
            this.tbProcessingAngle.Location = new System.Drawing.Point(102, 3);
            this.tbProcessingAngle.Name = "tbProcessingAngle";
            this.tbProcessingAngle.Size = new System.Drawing.Size(152, 20);
            this.tbProcessingAngle.TabIndex = 32;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 42;
            this.label5.Text = "Угол полосы";
            // 
            // tbDepth
            // 
            this.tbDepth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDepth.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bandsTechOperationBindingSource, "Depth", true));
            this.tbDepth.Location = new System.Drawing.Point(102, 154);
            this.tbDepth.Name = "tbDepth";
            this.tbDepth.Size = new System.Drawing.Size(152, 20);
            this.tbDepth.TabIndex = 47;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 157);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 41;
            this.label4.Text = "Глубина";
            // 
            // tbBandWidth
            // 
            this.tbBandWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBandWidth.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bandsTechOperationBindingSource, "BandWidth", true));
            this.tbBandWidth.Location = new System.Drawing.Point(102, 85);
            this.tbBandWidth.Name = "tbBandWidth";
            this.tbBandWidth.Size = new System.Drawing.Size(152, 20);
            this.tbBandWidth.TabIndex = 44;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 40;
            this.label3.Text = "Ширина полосы";
            // 
            // tbBandStart
            // 
            this.tbBandStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBandStart.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bandsTechOperationBindingSource, "BandStart", true));
            this.tbBandStart.Location = new System.Drawing.Point(102, 131);
            this.tbBandStart.Name = "tbBandStart";
            this.tbBandStart.Size = new System.Drawing.Size(152, 20);
            this.tbBandStart.TabIndex = 46;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 39;
            this.label2.Text = "Начало полосы";
            // 
            // tbBandSpacing
            // 
            this.tbBandSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBandSpacing.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bandsTechOperationBindingSource, "BandSpacing", true));
            this.tbBandSpacing.Location = new System.Drawing.Point(102, 108);
            this.tbBandSpacing.Name = "tbBandSpacing";
            this.tbBandSpacing.Size = new System.Drawing.Size(152, 20);
            this.tbBandSpacing.TabIndex = 45;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 38;
            this.label1.Text = "Расст.между пол.";
            // 
            // bCalculate
            // 
            this.bCalculate.Location = new System.Drawing.Point(0, 220);
            this.bCalculate.Name = "bCalculate";
            this.bCalculate.Size = new System.Drawing.Size(75, 23);
            this.bCalculate.TabIndex = 50;
            this.bCalculate.Text = "Расчитать";
            this.bCalculate.UseVisualStyleBackColor = true;
            this.bCalculate.Click += new System.EventHandler(this.bCalculate_Click);
            // 
            // gvPassList
            // 
            this.gvPassList.AllowUserToResizeRows = false;
            this.gvPassList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gvPassList.AutoGenerateColumns = false;
            this.gvPassList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gvPassList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gvPassList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gvPassList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.posDataGridViewTextBoxColumn,
            this.CuttingType});
            this.gvPassList.DataSource = this.passListBindingSource;
            this.gvPassList.Location = new System.Drawing.Point(0, 249);
            this.gvPassList.MultiSelect = false;
            this.gvPassList.Name = "gvPassList";
            this.gvPassList.Size = new System.Drawing.Size(257, 268);
            this.gvPassList.TabIndex = 60;
            // 
            // passListBindingSource
            // 
            this.passListBindingSource.DataMember = "PassList";
            this.passListBindingSource.DataSource = this.bandsTechOperationBindingSource;
            // 
            // tbMaxCrestWidth
            // 
            this.tbMaxCrestWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMaxCrestWidth.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bandsTechOperationBindingSource, "Depth", true));
            this.tbMaxCrestWidth.Location = new System.Drawing.Point(102, 190);
            this.tbMaxCrestWidth.Name = "tbMaxCrestWidth";
            this.tbMaxCrestWidth.Size = new System.Drawing.Size(152, 20);
            this.tbMaxCrestWidth.TabIndex = 48;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 193);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 13);
            this.label6.TabIndex = 47;
            this.label6.Text = "Макс.шир.гребня";
            // 
            // tbFeedFinishing
            // 
            this.tbFeedFinishing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFeedFinishing.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bandsTechOperationBindingSource, "FeedFinishing", true));
            this.tbFeedFinishing.Location = new System.Drawing.Point(102, 49);
            this.tbFeedFinishing.Name = "tbFeedFinishing";
            this.tbFeedFinishing.Size = new System.Drawing.Size(152, 20);
            this.tbFeedFinishing.TabIndex = 34;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 13);
            this.label7.TabIndex = 49;
            this.label7.Text = "Подача чистовая";
            // 
            // bUp
            // 
            this.bUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bUp.Location = new System.Drawing.Point(142, 220);
            this.bUp.Name = "bUp";
            this.bUp.Size = new System.Drawing.Size(53, 23);
            this.bUp.TabIndex = 61;
            this.bUp.Text = "Вверх";
            this.bUp.UseVisualStyleBackColor = true;
            this.bUp.Click += new System.EventHandler(this.bUp_Click);
            // 
            // bDown
            // 
            this.bDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bDown.Location = new System.Drawing.Point(201, 220);
            this.bDown.Name = "bDown";
            this.bDown.Size = new System.Drawing.Size(53, 23);
            this.bDown.TabIndex = 62;
            this.bDown.Text = "Вниз";
            this.bDown.UseVisualStyleBackColor = true;
            this.bDown.Click += new System.EventHandler(this.bDown_Click);
            // 
            // bandsTechOperationBindingSource
            // 
            this.bandsTechOperationBindingSource.DataSource = typeof(CAM.Tactile.BandsTechOperation);
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
            // BandsTechOperationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bDown);
            this.Controls.Add(this.bUp);
            this.Controls.Add(this.tbFeedFinishing);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbMaxCrestWidth);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.gvPassList);
            this.Controls.Add(this.bCalculate);
            this.Controls.Add(this.tbFeed);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.tbProcessingAngle);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbDepth);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbBandWidth);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbBandStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbBandSpacing);
            this.Controls.Add(this.label1);
            this.Name = "BandsTechOperationView";
            this.Size = new System.Drawing.Size(257, 517);
            ((System.ComponentModel.ISupportInitialize)(this.gvPassList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.passListBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bandsTechOperationBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbFeed;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbProcessingAngle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbDepth;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbBandWidth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbBandStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbBandSpacing;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.BindingSource bandsTechOperationBindingSource;
        private System.Windows.Forms.Button bCalculate;
        private System.Windows.Forms.DataGridView gvPassList;
        private System.Windows.Forms.BindingSource passListBindingSource;
        private System.Windows.Forms.TextBox tbMaxCrestWidth;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbFeedFinishing;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button bUp;
        private System.Windows.Forms.Button bDown;
        private System.Windows.Forms.DataGridViewTextBoxColumn posDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn CuttingType;
    }
}
