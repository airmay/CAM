using CAM.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace CAM
{
    /// <summary>
    /// Настройки
    /// </summary>
    public class Settings
    {
        #region static

        private static string GetFilePath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings.xml");

        /// <summary>
        /// Загрузить данные из файла в контейнер
        /// </summary>
        /// <returns></returns>
        public static Settings Load()
        {
            try
            {
                var formatter = new XmlSerializer(typeof(Settings));
                using (var fileStream = new FileStream(GetFilePath, FileMode.Open))
                    return (Settings)formatter.Deserialize(fileStream);
            }
            catch (Exception e)
            {
                Acad.Alert($"Ошибка при загрузке настроек из файла {GetFilePath}", e);
                return new Settings
                {
                    Tools = new List<Tool>(),
                    TechProcessParams = new TechProcessParams(),
                    SawingLineTechOperationParams = new SawingTechOperationParams(),
                    SawingCurveTechOperationParams = new SawingTechOperationParams()
                };
            }
        }

        /// <summary>
        /// Сохранить данные в файл
        /// </summary>
        public static void Save(Settings settings)
        {
            var formatter = new XmlSerializer(typeof(Settings));
            try
            {
                using (var fileStream = new FileStream(GetFilePath, FileMode.Create))
                    formatter.Serialize(fileStream, settings);
            }
            catch (Exception e)
            {
                Acad.Alert($"Ошибка при сохранении настроек в файл {GetFilePath}", e);
            }
        }

        #endregion

        #region Machines
        public Machine[] Machines { get; set; } = new Machine[]
        {
            new Machine
            {
                Type = MachineType.ScemaLogic,
                ProgramPath = @"\\US-CATALINA3\public\Программы станок\CodeRepository"
                // @"\\192.168.137.59\ssd\Automatico\";
            }
        };

        public Machine GetMachineSettings(MachineType type) => Machines.Single(p => p.Type == type);

        public class Machine
        {
            public MachineType Type { get; set; }

            public string ProgramPath { get; set; }
        }

        #endregion

        public List<Tool> Tools { get; set; }

        public TechProcessParams TechProcessParams { get; set; }

        //public Dictionary<ProcessingType, IProcessingParams> GetProcessingParams() => new Dictionary<ProcessingType, IProcessingParams> { { ProcessingType.Sawing, new SawingDefaultParams{
        //    SawingCurveTechOperationParams = this.SawingCurveTechOperationParams,
        //    SawingLineTechOperationParams = this.SawingLineTechOperationParams
        //} } };

        #region TechOperationParams

        public SawingTechOperationParams SawingLineTechOperationParams { get; set; }

        public SawingTechOperationParams SawingCurveTechOperationParams { get; set; }

//        public SawingDefaultParams SawingDefaultParams { get; set; }


        #endregion
    }
}
