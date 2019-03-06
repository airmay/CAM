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
			this.gbCalcParams = new System.Windows.Forms.GroupBox();
			this.bCalculate = new System.Windows.Forms.Button();
			this.edCompensation = new System.Windows.Forms.TextBox();
			this.lbCompensation = new System.Windows.Forms.Label();
			this.cbExactlyEnd = new System.Windows.Forms.CheckBox();
			this.sawingParamsBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.cbExactlyBegin = new System.Windows.Forms.CheckBox();
			this.gbSawingModes = new System.Windows.Forms.GroupBox();
			this.pSawingModesButton = new System.Windows.Forms.Panel();
			this.bLoadSawingModes = new System.Windows.Forms.Button();
			this.bSaveSawingModes = new System.Windows.Forms.Button();
			this.sawingModesView = new CAM.UI.SawingModesView();
			this.gbCalcParams.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sawingParamsBindingSource)).BeginInit();
			this.gbSawingModes.SuspendLayout();
			this.pSawingModesButton.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbCalcParams
			// 
			this.gbCalcParams.Controls.Add(this.bCalculate);
			this.gbCalcParams.Controls.Add(this.edCompensation);
			this.gbCalcParams.Controls.Add(this.lbCompensation);
			this.gbCalcParams.Controls.Add(this.cbExactlyEnd);
			this.gbCalcParams.Controls.Add(this.cbExactlyBegin);
			this.gbCalcParams.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbCalcParams.Location = new System.Drawing.Point(0, 0);
			this.gbCalcParams.Name = "gbCalcParams";
			this.gbCalcParams.Size = new System.Drawing.Size(245, 96);
			this.gbCalcParams.TabIndex = 0;
			this.gbCalcParams.TabStop = false;
			this.gbCalcParams.Text = "Расчетные параметры";
			// 
			// bCalculate
			// 
			this.bCalculate.Location = new System.Drawing.Point(6, 68);
			this.bCalculate.Name = "bCalculate";
			this.bCalculate.Size = new System.Drawing.Size(75, 23);
			this.bCalculate.TabIndex = 6;
			this.bCalculate.Text = "Рассчитать";
			this.bCalculate.UseVisualStyleBackColor = true;
			// 
			// edCompensation
			// 
			this.edCompensation.Location = new System.Drawing.Point(85, 42);
			this.edCompensation.Name = "edCompensation";
			this.edCompensation.Size = new System.Drawing.Size(50, 20);
			this.edCompensation.TabIndex = 5;
			// 
			// lbCompensation
			// 
			this.lbCompensation.AutoSize = true;
			this.lbCompensation.Location = new System.Drawing.Point(3, 45);
			this.lbCompensation.Name = "lbCompensation";
			this.lbCompensation.Size = new System.Drawing.Size(76, 13);
			this.lbCompensation.TabIndex = 4;
			this.lbCompensation.Text = "Компенсация";
			// 
			// cbExactlyEnd
			// 
			this.cbExactlyEnd.AutoSize = true;
			this.cbExactlyEnd.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.sawingParamsBindingSource, "IsExactlyEnd", true));
			this.cbExactlyEnd.Location = new System.Drawing.Point(122, 19);
			this.cbExactlyEnd.Name = "cbExactlyEnd";
			this.cbExactlyEnd.Size = new System.Drawing.Size(88, 17);
			this.cbExactlyEnd.TabIndex = 3;
			this.cbExactlyEnd.Text = "Конец точно";
			this.cbExactlyEnd.UseVisualStyleBackColor = true;
			// 
			// sawingParamsBindingSource
			// 
			this.sawingParamsBindingSource.DataSource = typeof(CAM.Domain.SawingTechOperationParams);
			this.sawingParamsBindingSource.DataSourceChanged += new System.EventHandler(this.sawingParamsBindingSource_DataSourceChanged);
			// 
			// cbExactlyBegin
			// 
			this.cbExactlyBegin.AutoSize = true;
			this.cbExactlyBegin.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.sawingParamsBindingSource, "IsExactlyBegin", true));
			this.cbExactlyBegin.Location = new System.Drawing.Point(6, 19);
			this.cbExactlyBegin.Name = "cbExactlyBegin";
			this.cbExactlyBegin.Size = new System.Drawing.Size(94, 17);
			this.cbExactlyBegin.TabIndex = 2;
			this.cbExactlyBegin.Text = "Начало точно";
			this.cbExactlyBegin.UseVisualStyleBackColor = true;
			// 
			// gbSawingModes
			// 
			this.gbSawingModes.Controls.Add(this.pSawingModesButton);
			this.gbSawingModes.Controls.Add(this.sawingModesView);
			this.gbSawingModes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbSawingModes.Location = new System.Drawing.Point(0, 96);
			this.gbSawingModes.Name = "gbSawingModes";
			this.gbSawingModes.Size = new System.Drawing.Size(245, 315);
			this.gbSawingModes.TabIndex = 2;
			this.gbSawingModes.TabStop = false;
			this.gbSawingModes.Text = "Режимы обработки";
			// 
			// pSawingModesButton
			// 
			this.pSawingModesButton.Controls.Add(this.bLoadSawingModes);
			this.pSawingModesButton.Controls.Add(this.bSaveSawingModes);
			this.pSawingModesButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pSawingModesButton.Location = new System.Drawing.Point(3, 283);
			this.pSawingModesButton.Name = "pSawingModesButton";
			this.pSawingModesButton.Size = new System.Drawing.Size(239, 29);
			this.pSawingModesButton.TabIndex = 2;
			// 
			// bLoadSawingModes
			// 
			this.bLoadSawingModes.Location = new System.Drawing.Point(84, 3);
			this.bLoadSawingModes.Name = "bLoadSawingModes";
			this.bLoadSawingModes.Size = new System.Drawing.Size(75, 23);
			this.bLoadSawingModes.TabIndex = 1;
			this.bLoadSawingModes.Text = "Загрузить";
			this.bLoadSawingModes.UseVisualStyleBackColor = true;
			// 
			// bSaveSawingModes
			// 
			this.bSaveSawingModes.Location = new System.Drawing.Point(3, 3);
			this.bSaveSawingModes.Name = "bSaveSawingModes";
			this.bSaveSawingModes.Size = new System.Drawing.Size(75, 23);
			this.bSaveSawingModes.TabIndex = 0;
			this.bSaveSawingModes.Text = "Сохранить";
			this.bSaveSawingModes.UseVisualStyleBackColor = true;
			// 
			// sawingModesView
			// 
			this.sawingModesView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sawingModesView.Location = new System.Drawing.Point(3, 16);
			this.sawingModesView.Name = "sawingModesView";
			this.sawingModesView.Size = new System.Drawing.Size(239, 296);
			this.sawingModesView.TabIndex = 1;
			// 
			// SawingParamsView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gbSawingModes);
			this.Controls.Add(this.gbCalcParams);
			this.Name = "SawingParamsView";
			this.Size = new System.Drawing.Size(245, 411);
			this.gbCalcParams.ResumeLayout(false);
			this.gbCalcParams.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sawingParamsBindingSource)).EndInit();
			this.gbSawingModes.ResumeLayout(false);
			this.pSawingModesButton.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbCalcParams;
        private System.Windows.Forms.CheckBox cbExactlyEnd;
        private System.Windows.Forms.CheckBox cbExactlyBegin;
        public System.Windows.Forms.BindingSource sawingParamsBindingSource;
        private SawingModesView sawingModesView;
        private System.Windows.Forms.Button bCalculate;
        private System.Windows.Forms.TextBox edCompensation;
        private System.Windows.Forms.Label lbCompensation;
        private System.Windows.Forms.GroupBox gbSawingModes;
        private System.Windows.Forms.Panel pSawingModesButton;
        private System.Windows.Forms.Button bLoadSawingModes;
        private System.Windows.Forms.Button bSaveSawingModes;
    }
}
