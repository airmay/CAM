using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using Autodesk.AutoCAD.Colors;
using Dreambuild.AutoCAD;

namespace CAM;

public static class ProcessingObjectBuilder
{
    private static DocumentLock _documentLock;
    private static Transaction _transaction;
    private static BlockTableRecord _currentSpace;
    private static ObjectId _layerId;
    //private static DBDictionary _groupDict;

    private static readonly Dictionary<string, Color> Colors = new()
    {
        [CommandNames.Cutting] = Color.FromColor(System.Drawing.Color.Green),
        [CommandNames.Uplifting] = Color.FromColor(System.Drawing.Color.Blue),
        [CommandNames.Penetration] = Color.FromColor(System.Drawing.Color.Yellow),
        [CommandNames.Fast] = Color.FromColor(System.Drawing.Color.Crimson),
        [CommandNames.Transition] = Color.FromColor(System.Drawing.Color.Yellow)
    };

    public static void Start()
    {
        _documentLock = Acad.ActiveDocument.LockDocument();
        _transaction = Acad.Database.TransactionManager.StartTransaction();
        _currentSpace = (BlockTableRecord)_transaction.GetObject(Acad.Database.CurrentSpaceId, OpenMode.ForWrite, false);

        _layerId = DbHelper.GetLayerId("Обработка");
        //_groupDict = (DBDictionary)_transaction.GetObject(Acad.Database.GroupDictionaryId, OpenMode.ForWrite);
    }

    public static ObjectId? AddToolpath(Curve curve, int? gCode = null)
    {
        if (!curve.IsNewObject) 
            return curve.ObjectId;
            
        if (curve.Length() < 1)
            return null;

        curve.Color = gCode.HasValue
            ? Colors[gCode == 0 ? CommandNames.Fast : CommandNames.Cutting]
            : Color.FromColor(System.Drawing.Color.DarkOliveGreen);

        return AddEntity(curve);
    }

    public static ObjectId AddEntity(Entity entity)
    {
        entity.LayerId = _layerId;
        _currentSpace.AppendEntity(entity);
        _transaction.AddNewlyCreatedDBObject(entity, true);

        return entity.ObjectId;
    }

    //public static ObjectId CreateGroup(string name, ObjectId[] entityIds)
    //{
    //    var group = new Group(name, false);
    //    group.Append(new ObjectIdCollection(entityIds));
    //    group.SetLayer(_layerId);
    //    var groupId = _groupDict.SetAt("*", group);
    //    _transaction.AddNewlyCreatedDBObject(group, true);

    //    return groupId;
    //}

    public static void Stop()
    {
        _transaction.Commit();
        _transaction.Dispose();
        _documentLock.Dispose();
    }
}