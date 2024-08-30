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
        IEnumerable<IOperation> Operations { get; }
        MachineType MachineType { get; set; }
        void Execute();
        void RemoveAcadObjects();
        void Init();
    }
}
