using CAM.CncWorkCenter;
using CAM.Core;
using System;
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
        void ExecutePartial(int programPosition);
    }

    [Serializable]
    public abstract class ProcessingBase<TTechProcess, TProcessor> : IProcessing
        where TTechProcess : ProcessingBase<TTechProcess, TProcessor>
        where TProcessor : ProcessorBase<TTechProcess, TProcessor>, new()
    {
        [NonSerialized] public TProcessor Processor;

        public string Caption { get; set; }
        public IOperation[] Operations { get; set; }
        public Tool Tool { get; set; }
        public short LastOperationNumber { get; set; }

        public Machine? Machine { get; set; }

        public Origin Origin { get; set; } = new Origin();

        public double ZSafety { get; set; } = 20;

        protected abstract bool Validate();

        public Program Execute()
        {
            var operations = Operations.Cast<OperationBase<TTechProcess, TProcessor>>().Where(p => p.Enabled).ToArray();
            if (!Validate() || operations.Length == 0 || operations.Any(p => !p.Validate()))
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

                return Processor.Program;
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
            Processor ??= new TProcessor { Processing = this as TTechProcess };
            
            //if (operations.Length == 3)
            //    throw new Exception("test");

            Processor.Start();
            foreach (var operation in operations)
            {
                Acad.Write($"расчет операции {operation.Caption}");
                Processor.Operation = operation;
                operation.SetProcessing(this);

                operation.Execute();
            }

            Processor.Finish();
        }

        public void ExecutePartial(int programPosition) => Processor.PartialProgram(programPosition);
    }
}