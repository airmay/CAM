using Autodesk.AutoCAD.DatabaseServices;
using CAM.Core;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CAM
{
    [Serializable]
    public class CamDocument
    {
        // https://adn-cis.org/serilizacziya-klassa-.net-v-bazu-chertezha-autocad.html или  https://www.rsdn.org/forum/dotnet/2900485.all
        private sealed class MyBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName) => Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
        }

        private const string DataKey = nameof(CamDocument);
        [NonSerialized] private int _hash;

        public IProcessing[] Processings { get; private set; }
        public int? ProcessingIndex { get; private set; }
        public string ProgramFileExtension { get; private set; }
        public Command[] Commands { get; private set; }

        public void Set(IProcessing[] processings, Program program)
        {
            Processings = processings;
            if (program != null)
            {
                ProcessingIndex = Array.IndexOf(processings, program.Processing);
                Commands = program.Commands.ToArray();
                ProgramFileExtension = program.ProgramFileExtension;
            }
        }

        public static CamDocument Create()
        {
            try
            {
                using (var tr = Acad.Database.TransactionManager.StartTransaction())
                using (var dict = tr.GetObject(Acad.Database.NamedObjectsDictionaryId, OpenMode.ForRead) as DBDictionary)
                    if (dict.Contains(DataKey))
                        using (var xRecord = tr.GetObject(dict.GetAt(DataKey), OpenMode.ForRead) as Xrecord)
                        using (var resultBuffer = xRecord.Data)
                        using (var stream = new MemoryStream())
                        {
                            foreach (var typedValue in resultBuffer)
                            {
                                var datachunk = Convert.FromBase64String((string)typedValue.Value);
                                stream.Write(datachunk, 0, datachunk.Length);
                            }

                            stream.Position = 0;
                            var formatter = new BinaryFormatter { Binder = new MyBinder() };

                            var camDocument = (CamDocument)formatter.Deserialize(stream);
                            camDocument._hash = resultBuffer.ToString().GetHashCode();

                            Array.ForEach(camDocument.Processings,
                                p => Array.ForEach(p.Operations, op => op.SetProcessing(p)));
#if DEBUG
                            Program.DwgFileCommands = camDocument.Commands;
#endif
                            return camDocument;
                        }
            }
            catch (Exception e)
            {
                Acad.Alert("Ошибка при загрузке данных обработки", e);
            }

            return new CamDocument();
        }

        public void Save()
        {
            try
            {
                const int kMaxChunkSize = 127;
                using (var resultBuffer = new ResultBuffer())
                {
                    using (var stream = new MemoryStream())
                    {
                        var formatter = new BinaryFormatter();
                        formatter.Serialize(stream, this);
                        stream.Position = 0;
                        for (var i = 0; i < stream.Length; i += kMaxChunkSize)
                        {
                            var length = (int)Math.Min(stream.Length - i, kMaxChunkSize);
                            var datachunk = new byte[length];
                            stream.Read(datachunk, 0, length);
                            resultBuffer.Add(new TypedValue((int)DxfCode.Text, Convert.ToBase64String(datachunk)));
                        }
                    }
                    var hash = resultBuffer.ToString().GetHashCode();
                    if (hash == _hash)
                        return;

                    using (var acLckDoc = Acad.ActiveDocument.LockDocument())
                    using (var tr = Acad.Database.TransactionManager.StartTransaction())
                    using (var dict = tr.GetObject(Acad.Database.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary)
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
                Acad.Alert("Ошибка при сохранении данных обработки", e);
            }
        }
    }
}
