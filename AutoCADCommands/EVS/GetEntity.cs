using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using AcRx = Autodesk.AutoCAD.Runtime;
using AcEd = Autodesk.AutoCAD.EditorInput;
using AcDb = Autodesk.AutoCAD.DatabaseServices;
using AcAp = Autodesk.AutoCAD.ApplicationServices;
using AcGe = Autodesk.AutoCAD.Geometry;
//[assembly: CommandClass(typeof(AutoCADCommands.EVS.GetEntity))]
namespace AutoCADCommands.EVS
{
    public class GetEntity
    {
        // Define Command "GetEnt"
        [CommandMethod("GetEnt")]
        static public void GetEnt()
        {
            AcEd.Editor ed = AcAp.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptResult rs = ed.GetString("\nВведите метку примитива (в hex-представлении): ");
            if (rs.Status != PromptStatus.OK) return;
            System.Int64 l = System.Int64.Parse(rs.StringResult, System.Globalization.NumberStyles.HexNumber);
            AcDb.Handle h = new AcDb.Handle(l);
            AcDb.Database db = AcDb.HostApplicationServices.WorkingDatabase;
            AcDb.ObjectId id = db.GetObjectId(false, h, 0);
            if (!id.IsValid)
            {
                ed.WriteMessage("\nОшибочная метка примитива {0}", h);
                return;
            }
            using (AcAp.DocumentLock doclock = ed.Document.LockDocument())
            {
                using (AcDb.Transaction tr = db.TransactionManager.StartTransaction())
                {
                    AcDb.Entity ent = tr.GetObject(id, AcDb.OpenMode.ForRead) as AcDb.Entity;
                    if (ent != null)
                    {
                        ed.WriteMessage("\nEntity Class: {0}", AcRx.RXClass.GetClass(ent.GetType()).Name);
                        // Ну и так далее - делаешь с примитивом то, что тебе нужно...
                    }
                    else
                    {
                        ed.WriteMessage("\nЭто не примитив!");
                    }
                }
            }
        }
    }
}