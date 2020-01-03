namespace CAM.TechOperations.Sawing
{
    [ParamsView(ProcessingType.Sawing)]
    public partial class SawingParamsView : ParamsView
    {
        public SawingParamsView()
        {
            InitializeComponent();
        }
		
	    public override void SetParams(object @params)
	    {
		    sawingParamsBindingSource.DataSource = @params;
            sawingParamsBindingSource.ResetBindings(false);
            sawingModesView.sawingModesBindingSource.DataSource = ((SawingTechOperationParams)sawingParamsBindingSource.DataSource).Modes;
	    }
	}
}
