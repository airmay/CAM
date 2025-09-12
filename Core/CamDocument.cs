using System;

namespace CAM
{
    public class CamDocument
    {
        [NonSerialized] private int _hash;

        public IProcessing[] Processings { get; set; }
        public int? ProcessingIndex { get; set; }
        public Command[] Commands { get; set; }

        public static CamDocument Create()
        {
            var document = new CamDocument();
            var (value, hash) = DataLoader.Load();
            if (value is IProcessing[] processings)
            {
                document.Processings = processings;
                document._hash = hash;
            }
            else if (value != null)
            {
                Acad.Alert("Ошибка при загрузке данных обработки");
            }

            return document;
        }

        public void Save(IProcessing[] processings, int? processingIndex, Command[] commands)
        {
            Processings = processings;
            ProcessingIndex = processingIndex;
            Commands = commands;

            DataLoader.Save(this, _hash);
        }
    }
}
