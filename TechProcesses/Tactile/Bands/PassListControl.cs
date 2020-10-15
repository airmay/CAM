using CAM.Tactile;
using System;
using System.Windows.Forms;

namespace CAM.TechProcesses.Tactile
{
    public partial class PassListControl : UserControl
    {
        public PassListControl(BindingSource bindingSource)
        {
            InitializeComponent();
            this.bindingSource.DataSource = bindingSource;
        }

        private void bUp_Click(object sender, EventArgs e)
        {
            if (bindingSource.List.SwapPrev(bindingSource.Position))
            {
                bindingSource.ResetBindings(false);
                bindingSource.Position--;
            }
        }

        private void bDown_Click(object sender, EventArgs e)
        {
            if (bindingSource.List.SwapNext(bindingSource.Position))
            {
                bindingSource.ResetBindings(false);
                bindingSource.Position++;
            }
        }

        private void bCalculate_Click(object sender, EventArgs e)
        {
            ((BandsTechOperation)((BindingSource)bindingSource.DataSource).DataSource).CalcPassList();
            bindingSource.ResetBindings(false);
        }
    }
}
