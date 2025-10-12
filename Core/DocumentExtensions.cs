using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CAM
{
    [Serializable]
    public static class DocumentExtensions
    {
        // https://adn-cis.org/serilizacziya-klassa-.net-v-bazu-chertezha-autocad.html или  https://www.rsdn.org/forum/dotnet/2900485.all
        private sealed class MyBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName) => Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
        }

        private const string DataKey = "CAM";
        private const string HashKey = "Hash";

        public static bool HasUserData(this Document document) => document.UserData.ContainsKey(DataKey);

        public static object GetUserData(this Document document) => document.UserData[DataKey];

        public static void SetUserData(this Document document, object data) => document.UserData[DataKey] = data;

        public static void LoadFromXrecord(this Document document)
        {
            object data = null;
            var hash = 0;
            try
            {
                using (var tr = document.Database.TransactionManager.StartTransaction())
                using (var dict = tr.GetObject(document.Database.NamedObjectsDictionaryId, OpenMode.ForRead) as DBDictionary)
                    if (dict.Contains(DataKey))
                        using (var xRecord = tr.GetObject(dict.GetAt(DataKey), OpenMode.ForRead) as Xrecord)
                        using (var resultBuffer = xRecord.Data)
                        {
                            using (var stream = new MemoryStream())
                            {
                                foreach (var typedValue in resultBuffer)
                                {
                                    var datachunk = Convert.FromBase64String((string)typedValue.Value);
                                    stream.Write(datachunk, 0, datachunk.Length);
                                }

                                stream.Position = 0;
                                var formatter = new BinaryFormatter { Binder = new MyBinder() };
                                data = formatter.Deserialize(stream);
                                hash = resultBuffer.ToString().GetHashCode();
                            }
                        }
            }
            catch (Exception e)
            {
                Acad.Alert("Ошибка при загрузке данных обработки", e);
            }

            document.UserData[DataKey] = data;
            document.UserData[HashKey] = hash;
        }

        public static void SaveToXrecord(this Document document, object data)
        {
            document.UserData[DataKey] = data;
            try
            {
                const int kMaxChunkSize = 127;
                using (var resultBuffer = new ResultBuffer())
                {
                    using (var stream = new MemoryStream())
                    {
                        var formatter = new BinaryFormatter();
                        formatter.Serialize(stream, document.UserData[DataKey]);
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
                    if (hash == (int)document.UserData[HashKey])
                        return;

                    document.UserData[HashKey] = hash;
                    using (var _ = document.LockDocument())
                    using (var tr = document.Database.TransactionManager.StartTransaction())
                    using (var dict = tr.GetObject(document.Database.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary)
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
