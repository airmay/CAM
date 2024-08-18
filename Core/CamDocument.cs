using System.Collections.Generic;

namespace CAM
{
    public class CamDocument
    {
        public int Hash;
        public Processing[] Processings { get; set; }

        public static CamDocument Create()
        {
            var document = new CamDocument();
            var (value, hash) = DataLoader.Load();
            if (value is Processing[] processings)
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

        public void Save(Processing[] processings)
        {
            Processings = processings;
            if (Processings.Length > 0 || Hash != 0)
                DataLoader.Save(Processings, Hash);
        }
    }
}
