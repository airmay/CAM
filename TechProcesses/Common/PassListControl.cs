﻿using System;
using System.Windows.Forms;

namespace CAM.TechProcesses.Tactile
{
    public partial class PassListControl : UserControl
    {
        public PassListControl(BindingSource bindingSource, bool showCalc = false)
        {
            InitializeComponent();
            this.BindingSource.DataSource = bindingSource;
            bCalculate.Visible = showCalc;
        }

        private void bUp_Click(object sender, EventArgs e)
        {
            if (BindingSource.List.SwapPrev(BindingSource.Position))
            {
                BindingSource.ResetBindings(false);
                BindingSource.Position--;
            }
        }

        private void bDown_Click(object sender, EventArgs e)
        {
            if (BindingSource.List.SwapNext(BindingSource.Position))
            {
                BindingSource.ResetBindings(false);
                BindingSource.Position++;
            }
        }

        private void bCalculate_Click(object sender, EventArgs e)
        {
            BindingSource.GetSource<BindingSource>().GetSource<BandsTechOperation>().CalcPassList();
            BindingSource.ResetBindings(false);
        }
    }
}
