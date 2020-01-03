using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace CAM
{
    static class TechProcessLoader
    {
        private const string DataKey = "TechProcessList";

        /// <summary>
        /// Загрузить технологические процессы из файла чертежа
        /// </summary>
        public static void LoadTechProsess(CamDocument camDocument, Settings settings)
        {
            try
            {
                using (Transaction tr = camDocument.Document.Database.TransactionManager.StartTransaction())
                using (DBDictionary dict = tr.GetObject(Acad.Database.NamedObjectsDictionaryId, OpenMode.ForRead) as DBDictionary)
                    if (dict.Contains(DataKey))
                        using (Xrecord xRecord = tr.GetObject(dict.GetAt(DataKey), OpenMode.ForRead) as Xrecord)
                        using (ResultBuffer resultBuffer = xRecord.Data)
                        using (MemoryStream stream = new MemoryStream())
                        {
                            camDocument.Hash = resultBuffer.ToString().GetHashCode();
                            foreach (var typedValue in resultBuffer)
                            {
                                var datachunk = Convert.FromBase64String((string)typedValue.Value);
                                stream.Write(datachunk, 0, datachunk.Length);
                            }
                            stream.Position = 0;
                            var formatter = new BinaryFormatter();
                            camDocument.TechProcessList = (List<TechProcess>)formatter.Deserialize(stream);
                        }

                if (camDocument.TechProcessList != null)
                {
                    camDocument.TechProcessList.ForEach(p => p.Init(settings));
                    Acad.Write($"Загружены техпроцессы: {string.Join(", ", camDocument.TechProcessList.Select(p => p.Name))}");
                }
            }
            catch (Exception e)
            {
                Acad.Alert($"Ошибка при загрузке техпроцессов", e);
            }
        }

        public static void SaveTechProsess(CamDocument camDocument)
        {
            if (camDocument.Hash == 0 && !camDocument.TechProcessList.Any()) return;
            try
            {
                const int kMaxChunkSize = 127;
                using (var resultBuffer = new ResultBuffer())
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, camDocument.TechProcessList);
                        stream.Position = 0;
                        for (int i = 0; i < stream.Length; i += kMaxChunkSize)
                        {
                            int length = (int)Math.Min(stream.Length - i, kMaxChunkSize);
                            byte[] datachunk = new byte[length];
                            stream.Read(datachunk, 0, length);
                            resultBuffer.Add(new TypedValue((int)DxfCode.Text, Convert.ToBase64String(datachunk)));
                        }
                    }
                    var newHash = resultBuffer.ToString().GetHashCode();
                    if (newHash == camDocument.Hash)
                        return;

                    using (DocumentLock acLckDoc = camDocument.Document.LockDocument())
                    using (Transaction tr = camDocument.Document.Database.TransactionManager.StartTransaction())
                    using (DBDictionary dict = tr.GetObject(camDocument.Document.Database.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary)
                    {
                        if (dict.Contains(DataKey))
                            using (var xrec = tr.GetObject(dict.GetAt(DataKey), OpenMode.ForWrite) as Xrecord)
                                xrec.Data = resultBuffer;
                        else
                            using (var xrec = new Xrecord { Data = resultBuffer })
                            {
                                dict.SetAt(DataKey, xrec);
                                tr.AddNewlyCreatedDBObject(xrec, true);
                                //xrec.ObjectClosed += new ObjectClosedEventHandler(OnDataModified);
                            }
                        tr.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                Acad.Alert($"Ошибка при сохранении техпроцессов", e);
            }
        }
    }
}
