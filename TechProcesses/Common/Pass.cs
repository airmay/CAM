using System;

namespace CAM.TechProcesses.Tactile
{
    [Serializable]
    public class Pass
    {
        public double Pos { get; set; }

        public string CuttingType { get; set; }

        public Pass() { }

        public Pass(double pos, string cuttingType)
        {
            Pos = pos.Round(3);
            CuttingType = cuttingType;
        }
    }
}
