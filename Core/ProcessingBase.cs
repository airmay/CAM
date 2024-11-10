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
        Program Execute();
        void HideToolpath(IOperation operation);
    }

    [Serializable]
    public abstract class ProcessingBase : IProcessing
    {
        public string Caption { get; set; }
        public IOperation[] Operations { get; set; }
        public abstract MachineType MachineType { get; }
        public abstract Program Program { get; }
        private IEnumerable<OperationBase> OperationsEnabled => Operations.Cast<OperationBase>().Where(p => p.Enabled);

        //public void Init()


        //{


        //    if (Origin != Point2d.Origin)


        //        OriginGroup = Acad.CreateOriginObject(Origin);


        //    foreach (var operation in Operations)


        //    {


        //        AcadObject.LoadAcadProps(operation);


        //        operation.Init();


        //    }


        //}


        //public void Teardown()


        //{


        //    foreach (var operation in Operations)


        //        operation.Teardown();


        //}


        //public void RemoveAcadObjects()


        //{


        //    foreach (var operation in Operations)


        //        operation.RemoveAcadObjects();


        //}

        protected abstract IProcessor CreateProcessor();

        protected abstract bool Validate();

        public Program Execute()
        {
            if (!Validate() || OperationsEnabled.Any(p => !p.Validate()))
                return null;

            Acad.Editor.UpdateScreen();
            Acad.Write($"Выполняется расчет обработки {Caption}");
            Acad.CreateProgressor("Расчет обработки");
            try
            {
                var stopwatch = Stopwatch.StartNew();
                ProcessOperations();
                stopwatch.Stop();
                Acad.Write($"Расчет обработки завершен {stopwatch.Elapsed}");

                return Program;
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

            return null;
        }

        protected virtual void ProcessOperations()
        {
            using (var processor = CreateProcessor())
            {
                processor.Start();
                foreach (var operation in OperationsEnabled)
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
            if (Operations == null)
                return;

            var objectIds = OperationsEnabled.Where(p => p.ProcessingArea != null).SelectMany(p => p.ProcessingArea.ObjectIds).ToArray();
            Acad.SelectObjectIds(objectIds);
            OperationsEnabled.ForAll(p => p.ToolpathGroupId?.SetGroupVisibility(true));
        }

        public void HideToolpath(IOperation operationToShow) => Operations?.ForAll(p => p.ToolpathGroupId?.SetGroupVisibility(p == operationToShow));
    }
}