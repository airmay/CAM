using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.Geometry;
using CAM.Core;
using System.Diagnostics;

namespace CAM
{
    [Serializable]
    public abstract class ProcessingBase : ProcessItem, IProcessing
    {
        public abstract IProgram Program { get; }
        protected abstract void ProcessOperations();

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

        public IProgram Execute()
        {
            Acad.Editor.UpdateScreen();
            Acad.Write($"Выполняется расчет обработки {Caption}");
            Acad.CreateProgressor("Расчет обработки");
            try
            {
                var stopwatch = Stopwatch.StartNew();
                ProcessOperations();
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
    }
}