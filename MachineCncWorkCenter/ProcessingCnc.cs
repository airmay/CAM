using System;

namespace CAM.CncWorkCenter
{
    [Serializable]
    public class ProcessingCnc : ProcessingBase<ProcessingCnc, ProcessorCnc>
    {
        public Material? Material { get; set; }

        public int Frequency { get; set; }

        public int CuttingFeed { get; set; }

        public int PenetrationFeed { get; set; }

        public static void ConfigureParamsView(ParamsControl view)
        {
            view.AddMachine(CAM.Machine.Donatoni, CAM.Machine.ScemaLogic, CAM.Machine.Forma);
            view.AddMaterial();
            view.AddIndent();
            view.AddTool();
            view.AddTextBox(nameof(Frequency));
            view.AddIndent();
            view.AddTextBox(nameof(CuttingFeed));
            view.AddTextBox(nameof(PenetrationFeed));
            view.AddIndent();
            view.AddOrigin();
            view.AddTextBox(nameof(ZSafety));
        }

        public ProcessingCnc()
        {
            Caption = "Обработка ЧПУ";
        }

        public PostProcessorCnc GetPostProcessor()
        {
            switch (Machine.Value)
            {
                case CAM.Machine.Donatoni:
                    return new DonatoniPostProcessor();
                case CAM.Machine.Krea:
                    return new DonatoniPostProcessor();
                case CAM.Machine.CableSawing:
                    return new DonatoniPostProcessor();
                case CAM.Machine.Forma:
                    return new DonatoniPostProcessor();
                case CAM.Machine.Champion:
                    return new DonatoniPostProcessor();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override bool Validate()
        {
            return Machine.CheckNotNull("Станок") && Tool.CheckNotNull("Инструмент");
        }

        //public void RemoveAcadObjects()
        //{
        //    foreach (var operation in Operations)
        //        operation.RemoveAcadObjects();
        //}
    }
}