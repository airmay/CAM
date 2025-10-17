using CAM.CncWorkCenter;
using CAM.Core;
using System;
using System.Diagnostics;
using System.Linq;

namespace CAM;

[Serializable]
public abstract class TechProcessBase<TTechProcess, TProcessor> : ITechProcess
    where TTechProcess : TechProcessBase<TTechProcess, TProcessor>
    where TProcessor : ProcessorBase<TTechProcess, TProcessor>, new()
{
    [NonSerialized] private TProcessor _processor;
    public TProcessor Processor => _processor ??= new TProcessor { TechProcess = this as TTechProcess };

    public string Caption { get; set; }
    public IOperation[] Operations { get; set; }
    public Machine? Machine { get; set; }
    public short LastOperationNumber { get; set; }
    public Tool Tool { get; set; }

    public Origin Origin { get; set; } = new();
    public double Delta { get; set; } = 5;
    public double ZSafety { get; set; } = 20;

    public Program Execute()
    {
        var operations = Operations.Cast<OperationBase<TTechProcess, TProcessor>>().Where(p => p.Enabled).ToArray();
        if (!ParamsValidator.Validate(Caption, this) || operations.Length == 0 || operations.Any(p => !ParamsValidator.Validate(p.Caption, p)))
            return null;

        Acad.Write($"Выполняется расчет обработки {Caption}");
        try
        {
            var stopwatch = Stopwatch.StartNew();

            Processor.Start();
            foreach (var operation in operations)
            {
                Acad.Write($"расчет операции {operation.Caption}");
                operation.TechProcess = this as TTechProcess;
                Processor.Operation = operation;
                operation.Execute();
            }

            Processor.Stop(); 
            stopwatch.Stop();
            Acad.Write($"Расчет обработки завершен {stopwatch.Elapsed}");

            return Processor.CreateProgram();
        }
        catch (Autodesk.AutoCAD.Runtime.Exception ex) when (ex.ErrorStatus == Autodesk.AutoCAD.Runtime.ErrorStatus.UserBreak)
        {
            Acad.Write("Расчет прерван");
        }
#if RELEASE
            catch (Exception ex)
            {
                Acad.Alert("Ошибка при выполнении расчета", ex);
            }
#endif
        return null;
    }

    public Program ExecutePartial(int position, IOperation operation, ToolPosition toolPosition)
    {
        return Processor.PartialProgram(position, operation, toolPosition);
    }
}