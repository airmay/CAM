using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.Colors;

namespace CAM
{
    public class ToolpathBuilder : IDisposable
    {
        private readonly DocumentLock _documentLock;
        private readonly Transaction _transaction;
        private readonly BlockTableRecord _currentSpace;
        private readonly ObjectId _layerId = Acad.GetProcessLayerId();
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
        }

        public ObjectId AddToolpath(Curve curve, string name)
        {
            if (!curve.IsNewObject) 
                return curve.ObjectId;

            if (_colors.TryGetValue(name, out var color))
                curve.Color = color;
            curve.LayerId = _layerId;
            curve.Visible = false;
            _currentSpace.AppendEntity(curve);
            _transaction.AddNewlyCreatedDBObject(curve, true);

            return curve.ObjectId;
        }

        public void Dispose()
        {
            _transaction.Commit();
            _transaction.Dispose();
            _documentLock.Dispose();
        }
    }
}
