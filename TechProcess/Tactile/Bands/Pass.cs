﻿using System;
using System.ComponentModel;

namespace CAM.Tactile
{
    [Serializable]
    public class Pass
    {
        public double Pos { get; set; }

        public string CuttingType { get; set; }

        public Pass() { }

        public Pass(double pos, string cuttingType)
        {
            Pos = pos;
            CuttingType = cuttingType;
        }
    }
}
