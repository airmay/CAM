using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace CAM
{
    public class CamDocument
    {
        private const string DataKey = "TechProcessList";
        
        private int _hash = 0;

        private readonly Document _document;

        public List<TechProcess> TechProcessList { get; set; } = new List<TechProcess>();

        public CamDocument(Document document)
        {
            _document = document;
            LoadTechProsess();
        }

        /// <summary>
        /// Загрузить технологические процессы из файла чертежа
        /// </summary>
        private void LoadTechProsess()
        {
            try
            {
                using (Transaction tr = _document.Database.TransactionManager.StartTransaction())
                using (DBDictionary dict = tr.GetObject(Acad.Database.NamedObjectsDictionaryId, OpenMode.ForRead) as DBDictionary)
                    if (dict.Contains(DataKey))
                        using (Xrecord xRecord = tr.GetObject(dict.GetAt(DataKey), OpenMode.ForRead) as Xrecord)
                        using (ResultBuffer resultBuffer = xRecord.Data)
                        using (MemoryStream stream = new MemoryStream())
                        {
                            _hash = resultBuffer.ToString().GetHashCode();
                            foreach (var typedValue in resultBuffer)
                            {
                                var datachunk = Convert.FromBase64String((string)typedValue.Value);
                                stream.Write(datachunk, 0, datachunk.Length);
                            }
                            stream.Position = 0;
                            var formatter = new BinaryFormatter();
                            TechProcessList = (List<TechProcess>)formatter.Deserialize(stream);
                        }

                if (TechProcessList != null)
                {
                    TechProcessList.ForEach(tp =>
                        tp.TechOperations.ForEach(to =>
                        {
                            to.ProcessingArea.AcadObjectId = Acad.GetObjectId(to.ProcessingArea.Handle);
                            to.TechProcess = tp;
                        }));
                    Acad.Write($"Загружены техпроцессы: {string.Join(", ", TechProcessList.Select(p => p.Name))}");
                }
            }
            catch (Exception e)
            {
                Acad.Alert($"Ошибка при загрузке техпроцессов", e);
            }
        }

        public void SaveTechProsess()
        {
            if (_hash == 0 && !TechProcessList.Any()) return;
            try
            {
                const int kMaxChunkSize = 127;
                using (var resultBuffer = new ResultBuffer())
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, TechProcessList);
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
                    if (newHash == _hash) return;

                    using (DocumentLock acLckDoc = _document.LockDocument())
                    using (Transaction tr = _document.Database.TransactionManager.StartTransaction())
                    using (DBDictionary dict = tr.GetObject(_document.Database.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary)
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
