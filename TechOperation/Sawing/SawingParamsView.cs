namespace CAM.TechOperation.Sawing
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
            var techOperation = (SawingTechOperation)@params;

            sawingParamsBindingSource.DataSource = techOperation.Params;
            sawingParamsBindingSource.ResetBindings(false);
            sawingModesView.sawingModesBindingSource.DataSource = techOperation.SawingParams.Modes;
	    }
	}
}
