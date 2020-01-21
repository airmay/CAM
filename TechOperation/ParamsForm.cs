using System.Windows.Forms;

namespace CAM.TechOperation
{
    public partial class ParamsForm : Form
    {
        public ParamsForm()
        {
            InitializeComponent();
        }

        public void SetParams(ProcessingType processingType, object @params, string text)
        {
            ParamsViewContainer.SetDefaultParamsView(processingType, @params, pParams);
            Text = $"Параметры операции \"{text}\"";
        }
    }
}
