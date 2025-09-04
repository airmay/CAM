using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.Colors;
using Dreambuild.AutoCAD;
using System.Xml.Linq;

namespace CAM
{
    public class ToolpathBuilder : IDisposable
    {
        private readonly DocumentLock _documentLock;
        private readonly Transaction _transaction;
        private readonly BlockTableRecord _currentSpace;
        private readonly ObjectId _layerId;
        private readonly DBDictionary _groupDict;
        private Group _group;


        private readonly Dictionary<string, Color> _colors = new Dictionary<string, Color>
        {
            [CommandNames.Cutting] = Color.FromColor(System.Drawing.Color.Green),
            [CommandNames.Uplifting] = Color.FromColor(System.Drawing.Color.Blue),
            [CommandNames.Penetration] = Color.FromColor(System.Drawing.Color.Yellow),
            [CommandNames.Fast] = Color.FromColor(System.Drawing.Color.Crimson),
            [CommandNames.Transition] = Color.FromColor(System.Drawing.Color.Yellow)
        };

        public ToolpathBuilder()
        {
            _documentLock = Acad.ActiveDocument.LockDocument();
            _transaction = Acad.Database.TransactionManager.StartTransaction();
            _currentSpace = (BlockTableRecord)_transaction.GetObject(Acad.Database.CurrentSpaceId, OpenMode.ForWrite, false);

            _layerId = DbHelper.GetLayerId(Acad.ProcessLayerName);
            _groupDict = (DBDictionary)_transaction.GetObject(Acad.Database.GroupDictionaryId, OpenMode.ForWrite);
        }

        public ObjectId? AddToolpath(Curve curve, int? gCode = null)
        {
            if (!curve.IsNewObject) 
                return curve.ObjectId;
            
            if (curve.Length() < 1)
                return null;

            curve.Color = gCode.HasValue
                ? _colors[gCode == 0 ? CommandNames.Fast : CommandNames.Cutting]
                : Color.FromColor(System.Drawing.Color.DarkOliveGreen);

            var id = AddEntity(curve);
            _group.Append(id);
            
            return id;
        }

        public ObjectId AddEntity(Entity entity)
        {
            entity.LayerId = _layerId;
            _currentSpace.AppendEntity(entity);
            _transaction.AddNewlyCreatedDBObject(entity, true);

            return entity.ObjectId;
        }

        public void CreateGroup() => _group = new Group("*", false);

        public ObjectId AddGroup(string name)
        {
            _group.SetLayer(_layerId);
            var groupId = _groupDict.SetAt("*", _group);
            _transaction.AddNewlyCreatedDBObject(_group, true);
            return groupId;
        }

        public void Dispose()
        {
            _transaction.Commit();
            _transaction.Dispose();
            _documentLock.Dispose();
        }
    }
}
