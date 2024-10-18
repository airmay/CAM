using System;
using System.Linq;
using CAM.Core;
using System.Diagnostics;

namespace CAM
{
    [Serializable]
    public abstract class ProcessingBase : ProcessItem
    {
        public abstract Program Program { get; }
        protected abstract void ProcessOperations();

        protected ProcessingBase()
        {
            Enabled = true;
        }
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

        public Program Execute()
        {
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
            Acad.CloseProgressor();

            return Program;
        }

        private void CreateToolpathGroups()
        {
            foreach (var operationGroup in Program.Commands.Where(p => p.Operation != null).GroupBy(p => p.Operation))
                operationGroup.Key.ToolpathGroup = operationGroup.Select(p => p.ObjectId).CreateGroup();
        }

        public void UpdateCaptions()
        {
            Caption = GetCaption(Caption, Children.Cast<Operation>().Sum(p => p.Duration));
            foreach (Operation operation in Children)
                operation.Caption = GetCaption(operation.Caption, operation.Duration);

            return;

            string GetCaption(string caption, double duration)
            {
                var ind = caption.IndexOf('(');
                var timeSpan = new TimeSpan(0, 0, 0, (int)duration);
                return $"{(ind > 0 ? caption.Substring(0, ind).Trim() : caption)} ({timeSpan})";
            }
        }
    }
}