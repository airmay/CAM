using System;
using System.Collections.Generic;
using System.Linq;
using CAM.Core;
using System.Diagnostics;

namespace CAM
{
    [Serializable]
    public abstract class ProcessingBase : ProcessItem
    {
        public abstract Program Program { get; }
        private IEnumerable<OperationBase> OperationsEnabled => Children.Cast<OperationBase>().Where(p => p.Enabled);

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
            OperationsEnabled.ForAll(p => p.Clear());
            if (!Validate() || OperationsEnabled.Any(p => !p.Validate()))
                return null;

            Acad.Editor.UpdateScreen();
            Acad.Write($"Выполняется расчет обработки {Caption}");
            Acad.CreateProgressor("Расчет обработки");
            try
            {
                var stopwatch = Stopwatch.StartNew();
                ProcessOperations();

                CreateToolpathGroups();
                UpdateCaptions();

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
                    processor.Operation = operation;
                    operation.ProcessingBase = this;
                    operation.Execute();
                }

                processor.Finish();
            }
        }

        private void CreateToolpathGroups()
        {
            foreach (var operationGroup in Program.ArraySegment.Where(p => p.Operation != null).GroupBy(p => p.Operation))
                operationGroup.Key.ToolpathGroup = operationGroup.Select(p => p.ObjectId).CreateGroup();
        }

        public void UpdateCaptions()
        {
            Caption = GetCaption(Caption, OperationsEnabled.Sum(p => p.Duration));
            foreach (var operation in OperationsEnabled)
                operation.Caption = GetCaption(operation.Caption, operation.Duration);

            return;

            string GetCaption(string caption, double duration)
            {
                var ind = caption.IndexOf('(');
                var timeSpan = new TimeSpan(0, 0, 0, (int)duration);
                return $"{(ind > 0 ? caption.Substring(0, ind).Trim() : caption)} ({timeSpan})";
            }
        }

        public override void OnSelect()
        {
            var objectIds = OperationsEnabled.Where(p => p.ProcessingArea != null).SelectMany(p => p.ProcessingArea.ObjectIds).ToArray();
            Acad.SelectObjectIds(objectIds);
            OperationsEnabled.ForAll(p => p.ToolpathGroup?.SetGroupVisibility(true));
        }

        public void HideToolpath(OperationBase operationToShow)
        {
            Children.Cast<OperationBase>().ForAll(p => p.ToolpathGroup?.SetGroupVisibility(p == operationToShow));
        }
    }
}