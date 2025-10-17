using CAM.Autocad;
using CAM.Core.Tools;

namespace CAM.Core;

public interface IOperation
{
    string Caption { get; set; }
    bool Enabled { get; set; }
    short Number { get; set; }
    Tool GetTool();
    AcadObject ProcessingArea { get; }
}