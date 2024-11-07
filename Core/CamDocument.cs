namespace CAM
{
    public class CamDocument
    {
        public int Hash;
        public IProcessing[] Processings { get; set; }

        public static CamDocument Create()
        {
            var document = new CamDocument();
            var (value, hash) = DataLoader.Load();
            if (value is IProcessing[] processings)
            {
                document.Processings = processings;
                document.Hash = hash;
                foreach (var processing in processings)
                foreach (var operation in processing.Operations)
                    operation.ProcessingBase = processing;
            }
            else if (value != null)
            {
                Acad.Alert("Ошибка при загрузке данных обработки");
            }

            return document;
        }

        public void Save(IProcessing[] processings)
        {
            Processings = processings;
            DataLoader.Save(Processings, Hash);
        }
    }
}
