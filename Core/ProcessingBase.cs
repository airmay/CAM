using System;
using System.Collections.Generic;
using System.Linq;
using CAM.Core;
using System.Diagnostics;

namespace CAM
{
    public interface IProcessing : ITreeNode
    {
        IOperation[] Operations { get; set; }
        MachineType MachineType { get; }
        Machine? Machine { get; set; }
        void Execute();
        void HideToolpath(IOperation operation);
    }

    [Serializable]
    public abstract class ProcessingBase : IProcessing
    {
        public string Caption { get; set; }
        public IOperation[] Operations { get; set; }
        public int LastOperationNumber { get; set; }
        public Machine? Machine { get; set; }
        public Origin Origin { get; set; } = new Origin();
        public double ZSafety { get; set; } = 20;
        public abstract MachineType MachineType { get; }

        protected abstract IProcessor CreateProcessor();

        protected abstract bool Validate();

        public void Execute()
        {
            var operations = Operations.Cast<OperationBase>().Where(p => p.Enabled).ToArray();
            if (!Validate() || operations.Length == 0 || operations.Any(p => !p.Validate()))
                return;

            Acad.Editor.UpdateScreen();
            Acad.Write($"Выполняется расчет обработки {Caption}");
            Acad.CreateProgressor("Расчет обработки");
            try
            {
                var stopwatch = Stopwatch.StartNew();
                ProcessOperations(operations);
                stopwatch.Stop();
                Acad.Write($"Расчет обработки завершен {stopwatch.Elapsed}");

                return;
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex) when (ex.ErrorStatus == Autodesk.AutoCAD.Runtime.ErrorStatus.UserBreak)
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
        }

        protected virtual void ProcessOperations(OperationBase[] operations)
        {
            using (var processor = CreateProcessor())
            {
                processor.Start();
                foreach (var operation in operations)
                {
                    Acad.Write($"расчет операции {operation.Caption}");
                    processor.SetOperation(operation);
                    operation.ProcessingBase = this;
                    operation.Execute();
                }

                processor.Finish();
            }
        }

        public void OnSelect()
        {
            Operations?.ForAll(p => p.ToolpathGroupId?.SetGroupVisibility(true));
            Acad.SelectObjectIds();
        }

        public void HideToolpath(IOperation operationToShow) => Operations?.ForAll(p => p.ToolpathGroupId?.SetGroupVisibility(p == operationToShow));
    }
}