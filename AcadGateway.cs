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
    public class AcadGateway //: IAcadGateway
    {
        private static AcadGateway _instance;

        public static AcadGateway Instance { get => _instance ?? (_instance = new AcadGateway()); }

        public Document Document => Application.DocumentManager.MdiActiveDocument;

        public Database Database => Application.DocumentManager.MdiActiveDocument.Database;

        public Editor Editor => Application.DocumentManager.MdiActiveDocument.Editor;

        public void WriteMessage(string message) => Editor.WriteMessage($"{message}\n");

        public ObjectId GetObjectId(long handle) => Database.GetObjectId(false, new Handle(handle), 0);

        public void SaveCurves(IEnumerable<Curve> entities)
        {
            App.LockAndExecute(() =>
            {
                string layer = "Обработка";
                var layerId = DbHelper.GetLayerId(layer);
                entities.Select(p => { p.LayerId = layerId; return p; }).AddToCurrentSpace();
            });
        }

        public void DeleteCurves(IEnumerable<Curve> idList)
        {
            if (idList?.Any() == true)
                App.LockAndExecute(() => idList.Select(p => p.ObjectId).QForEach(p => p.Erase()));
        }

        public IEnumerable<Curve> GetSelectedCurves() => Interaction.GetPickSet().QOpenForRead<Curve>();

        public void SelectCurve(ObjectId objectId) => SelectCurves(new ObjectId[] { objectId });

        private ObjectId[] _highlightedObjects;

        public void SelectCurves(ObjectId[] objectIds)
        {
            App.LockAndExecute(() =>
            {
                if (_highlightedObjects != null)
                    Interaction.UnhighlightObjects(_highlightedObjects);
                Interaction.HighlightObjects(objectIds);
                _highlightedObjects = objectIds;
            });
            Editor.UpdateScreen();
        }

        #region XRecord methods
        public object LoadDocumentData(string dataKey)
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

        public void SaveDocumentData(object data, string dataKey)
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
