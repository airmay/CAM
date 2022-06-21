using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace AutoCADCommands.EVS
{
    class GetEntityType
    {
        [CommandMethod("TestTextChange")]
        public static void TestTextChange()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord btr = (BlockTableRecord) tr.GetObject
                    (SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);

                foreach (ObjectId id in btr)
                {
                    Entity currentEntity = tr.GetObject(id, OpenMode.ForWrite, false) as Entity;
                    if (currentEntity == null)
                    {
                        continue;
                    }

                    if (currentEntity.GetType() == typeof(MText))
                    {
                        ((MText) currentEntity).Contents = "BlahBlah";
                    }
                    else
                    {
                        ((DBText) currentEntity).TextString = "BlahBlah";
                    }
                }

                tr.Commit();
            }


        }
    }
}
