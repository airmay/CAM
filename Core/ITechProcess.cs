using CAM.Core;

namespace CAM;

public interface ITechProcess
{
    string Caption { get; set; }
    IOperation[] Operations { get; set; }
    Machine? Machine { get; }
    short LastOperationNumber { get; set; }
    public Tool Tool { get; }
    Program Execute();
    Program ExecutePartial(int position, IOperation operationNumber, ToolPosition toolPosition);
}