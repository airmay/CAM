using CAM.Core;

namespace CAM
{
    public class CamDocument
    {
        public int Hash;
        public ProcessItem[] ProcessItems { get; set; }

        public static CamDocument Create()
        {
            var document = new CamDocument();
            var (value, hash) = DataLoader.Load();
            if (value is ProcessItem[] processItems)
            {
                document.ProcessItems = processItems;
                document.Hash = hash;
                //foreach (var processing in processItems)
                //    processing.Init(); // TODO
            }
            else if (value != null)
            {
                Acad.Alert("Ошибка при загрузке данных обработки");
            }

            return document;
        }

        public void Save(ProcessItem[] processItems)
        {
            ProcessItems = processItems;
            DataLoader.Save(ProcessItems, Hash);
        }
    }
}
