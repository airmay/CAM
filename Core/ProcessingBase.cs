using CAM.CncWorkCenter;
using CAM.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CAM
{
    public interface IProcessing
    {
        string Caption { get; set; }
        IOperation[] Operations { get; set; }
        Machine? Machine { get; set; }
        Program Execute();
        Origin Origin { get; set; }
        short LastOperationNumber { get; set; }
        Program ExecutePartial(int position, int count, short operationNumber, ToolPosition toolPosition);
        IOperation GetOperation(short number);
    }

    [Serializable]
    public abstract class ProcessingBase<TTechProcess, TProcessor> : IProcessing
        where TTechProcess : ProcessingBase<TTechProcess, TProcessor>
        where TProcessor : ProcessorBase<TTechProcess, TProcessor>, new()
    {
        [NonSerialized] private TProcessor _processor;
        public TProcessor Processor => _processor ??= new TProcessor { Processing = this as TTechProcess };

        public string Caption { get; set; }
        public IOperation[] Operations { get; set; }
        public Tool Tool { get; set; }
        public short LastOperationNumber { get; set; }

        public Machine? Machine { get; set; }

        public Origin Origin { get; set; } = new Origin();

        public double ZSafety { get; set; } = 20;

        protected abstract bool Validate();
        public IOperation GetOperation(short number) => Operations.FirstOrDefault(p => p.Number == number);

        public Program Execute()
        {
            var operations = Operations.Cast<OperationBase<TTechProcess, TProcessor>>().Where(p => p.Enabled).ToArray();

            if (!ParamsValidator.Validate(Caption, this) || operations.Length == 0 || operations.Any(p => !ParamsValidator.Validate(p.Caption, p)))
                return null;

            Acad.Editor.UpdateScreen();
            Acad.Write($"Выполняется расчет обработки {Caption}");
            Acad.CreateProgressor("Расчет обработки");
            try
            {
                var stopwatch = Stopwatch.StartNew();

                ProcessOperations(operations);
                stopwatch.Stop();
                Acad.Write($"Расчет обработки завершен {stopwatch.Elapsed}");

                return Processor.CreateProgram();
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex) when (ex.ErrorStatus ==
                                                                Autodesk.AutoCAD.Runtime.ErrorStatus.UserBreak)
            {
                Acad.Write("Расчет прерван");
            }
#if !DEBUG
            catch (Exception ex)
            {
                Acad.Alert("Ошибка при выполнении расчета", ex);
            }
#endif
            finally
            {
                Acad.CloseProgressor();
            }

            return null;
        }

        private void ProcessOperations(OperationBase<TTechProcess, TProcessor>[] operations)
        {
            Processor.Start();
            foreach (var operation in operations)
            {
                Acad.Write($"расчет операции {operation.Caption}");
                Processor.Operation = operation;

                operation.Execute();
            }
            Processor.Stop();
        }

        public Program ExecutePartial(int position, int count, short operationNumber, ToolPosition toolPosition) =>
            Processor.PartialProgram(position, count, operationNumber, toolPosition);
    }
}