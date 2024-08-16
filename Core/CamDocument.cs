using System.Collections.Generic;

namespace CAM
{
    public class CamDocument
    {
        public int Hash;
        public List<Processing> Processings { get; private set; } = new List<Processing>();

        public static CamDocument Create()
        {
            var document = new CamDocument();
            var (value, hash) = DataLoader.Load();
            if (value is List<Processing> processings)
            {
                document.Processings = processings;
                document.Hash = hash;
                foreach (var processing in processings)
                    processing.Init();
            }
            else if (value != null)
            {
                Acad.Alert("Ошибка при загрузке данных обработки");
            }

            return document;
        }


        public void Save()
        {
            if (Processings.Count > 0 || Hash != 0)
                DataLoader.Save(Processings, Hash);
        }
    }
}
