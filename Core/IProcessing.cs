﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Core
{
    public interface IProcessing
    {
        string Caption { get; set; }
        List<OperationCnc> Operations { get; set; }
        MachineType MachineType { get; set; }
        void Execute();
        void RemoveAcadObjects();
        void Init();
    }
}
