using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Core
{
    public interface IProcessing
    {
        string Caption { get; set; }
        OperationCnc[] Operations { get; set; }
        void Execute();
        void RemoveAcadObjects();
        void Init();
        OperationCnc CreateOperation(Type type, OperationCnc prototype);
    }
}
