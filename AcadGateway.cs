using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace CAM
{
    /// <summary>
    /// Класс осуществляющий взаимодействие с автокадом
    /// </summary>
    public static class Acad
    {
        public static Document Document => Application.DocumentManager.MdiActiveDocument;

        public static Database Database => Application.DocumentManager.MdiActiveDocument.Database;

        public static Editor Editor => Application.DocumentManager.MdiActiveDocument.Editor;

        public static void WriteMessage(string message) => Editor.WriteMessage($"{message}\n");

        public static ObjectId GetObjectId(long handle) => Database.GetObjectId(false, new Handle(handle), 0);

        public static void SaveCurves(IEnumerable<Curve> entities)
        {
            App.LockAndExecute(() =>
            {
                string layer = "Обработка";
                var layerId = DbHelper.GetLayerId(layer);
                entities.Select(p => { p.LayerId = layerId; return p; }).AddToCurrentSpace();
            });
        }

        public static void DeleteCurves(IEnumerable<Curve> curves)
        {
            var ids = curves.Select(p => p.ObjectId).ToArray();
            if (ids.Any())
            {
                _highlightedObjects = _highlightedObjects.Except(ids).ToArray();
                App.LockAndExecute(() => ids.QForEach(p => p.Erase()));
            }
        }

        public static IEnumerable<Curve> GetSelectedCurves() => OpenForRead(Interaction.GetPickSet());

        public static Curve[] OpenForRead(IEnumerable<ObjectId> ids)
        {
            return ids.Any() ? ids.QOpenForRead<Curve>() : Array.Empty<Curve>();
        }

        private static ObjectId[] _highlightedObjects = Array.Empty<ObjectId>();

        public static void SelectCurve(Curve curve)
        {
            if (curve != null)
                SelectObjectIds(curve.ObjectId);
            else
                SelectObjectIds();
        }

        public static void SelectObjectIds(params ObjectId[] objectIds)
        {
            App.LockAndExecute(() =>
            {
                if (_highlightedObjects.Any())
                    Interaction.UnhighlightObjects(_highlightedObjects);
                if (objectIds.Any())
                    Interaction.HighlightObjects(objectIds);
                _highlightedObjects = objectIds;
            });
            Editor.UpdateScreen();
        }

        #region XRecord methods
        public static object LoadDocumentData(string dataKey)
        {
            using (Transaction tr = Database.TransactionManager.StartTransaction())
            using (DBDictionary dict = tr.GetObject(Database.NamedObjectsDictionaryId, OpenMode.ForRead) as DBDictionary)
                if (dict.Contains(dataKey))
                    using (Xrecord xRecord = tr.GetObject(dict.GetAt(dataKey), OpenMode.ForRead) as Xrecord)
                    using (ResultBuffer resultBuffer = xRecord.Data)
                    using (MemoryStream stream = new MemoryStream())
                    {
                        foreach (var typedValue in resultBuffer)
                        {
                            var datachunk = Convert.FromBase64String((string)typedValue.Value);
                            stream.Write(datachunk, 0, datachunk.Length);
                        }
                        stream.Position = 0;
                        var formatter = new BinaryFormatter();
                        return formatter.Deserialize(stream);
                    }
            return null;
        }

        public static void SaveDocumentData(object data, string dataKey)
        {
            const int kMaxChunkSize = 127;
            using (var resultBuffer = new ResultBuffer())
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, data);
                    stream.Position = 0;
                    for (int i = 0; i < stream.Length; i += kMaxChunkSize)
                    {
                        int length = (int)Math.Min(stream.Length - i, kMaxChunkSize);
                        byte[] datachunk = new byte[length];
                        stream.Read(datachunk, 0, length);
                        resultBuffer.Add(new TypedValue((int)DxfCode.Text, Convert.ToBase64String(datachunk)));
                    }
                }

                using (DocumentLock acLckDoc = Document.LockDocument())
                using (Transaction tr = Database.TransactionManager.StartTransaction())
                using (DBDictionary dict = tr.GetObject(Database.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary)
                {
                    if (dict.Contains(dataKey))
                        using (var xrec = tr.GetObject(dict.GetAt(dataKey), OpenMode.ForWrite) as Xrecord)
                            xrec.Data = resultBuffer;
                    else
                        using (var xrec = new Xrecord { Data = resultBuffer })
                        {
                            dict.SetAt(dataKey, xrec);
                            tr.AddNewlyCreatedDBObject(xrec, true);
                            //xrec.ObjectClosed += new ObjectClosedEventHandler(OnDataModified);
                        }
                    tr.Commit();
                }
            }
        } 
        #endregion

    }
}
