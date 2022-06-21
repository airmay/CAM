using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCADCommands.EVS
{
    class NevedomayFigny
    {
        [CommandMethod("NevedomayFignya", CommandFlags.UsePickSet)]
        public void NevedomayFignya()
        {
            // Идентификатор выбранного примитива
            ObjectId lSelectObjectId = ObjectId.Null;
            // Запрашиваем текущий документ, его БД и редактор
            Document lAcadDoc = Application.DocumentManager.MdiActiveDocument;
            Database lAcadDb = lAcadDoc.Database;
            Editor lAcadEd = lAcadDoc.Editor;



            // Получаем набор предварительного выбора
            PromptSelectionResult lPromptSelectionResult = lAcadEd.SelectImplied();
            // Если выбор успешен
            if (lPromptSelectionResult.Status == PromptStatus.OK)
            {
                // Если выбран один примитив
                if (lPromptSelectionResult.Value.Count == 1)
                {
                    // Получаем его идентификатор
                    lSelectObjectId = lPromptSelectionResult.Value[0].ObjectId;
                }
                else
                {
                    // Выводим сообщение
                    lAcadEd.WriteMessage("\nВыделено более одного примитива на чертеже!\n");
                }
            }



            // Если идентификатор еще не определен
            if (lSelectObjectId.IsNull)
            {
                // Запрашиваем примитив 
                PromptEntityResult lPromptEntityResult = lAcadEd.GetEntity("\nВыберите примитив:");
                // Если пользователь выбрал 
                if (lPromptEntityResult.Status == PromptStatus.OK) lSelectObjectId = lPromptEntityResult.ObjectId;
            }
            // Если был выбран примитив любым способом
            if (!lSelectObjectId.IsNull)
            {
                // Выводим сообщение
                lAcadEd.WriteMessage("\nВыбран примитив с Id = [{0}]", lSelectObjectId.ToString());
            }
            else
            {
                // Выводим сообщение
                lAcadEd.WriteMessage("\nПримитив не был выбран!");
            }
        }



    }
}
