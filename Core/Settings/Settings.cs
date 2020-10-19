using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace CAM
{
    /// <summary>
    /// Настройки
    /// </summary>
    public class Settings
    {
        #region static func

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
                throw;
                //settings = new Settings
                //{
                //    //Tools = new List<Tool>(),
                //    //TechProcessParams = new TechProcessParams(),
                //    SawingLineTechOperationParams = new SawingTechOperationParams(),
                //    SawingCurveTechOperationParams = new SawingTechOperationParams()
                //};
            }
            //if (!settings.MachineSettingsList.Any())
            //{
            //    settings.MachineSettingsList = new List<MachineSettings>()
            //    {
            //        new MachineSettings{ MachineType = MachineType.ScemaLogic, Tools = new List<Tool>(), MaxFrequency = 3000, ZSafety = 20 },
            //        new MachineSettings{ MachineType = MachineType.Donatoni, Tools = new List<Tool>(), MaxFrequency = 5000, ZSafety = 20 },
            //        new MachineSettings{ MachineType = MachineType.Krea, Tools = new List<Tool>(), MaxFrequency = 10000, ZSafety = 20 }
            //    };
            //}
            //if (settings.TactileTechProcessParams == null)
            //{
            //    settings.TactileTechProcessParams = Tactile.TactileTechProcessParams.GetDefault();
            //}
            //return settings;
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

        #region Tools
        public List<Tool> ToolsScemaLogic { get; set; }

        public List<Tool> ToolsDonatoni { get; set; }

        public List<Tool> ToolsKrea { get; set; } 
        #endregion

        public List<MachineSettings> MachineSettings { get; set; }

        #region TechProcessParams
        public CAM.TechProcesses.Sawing.SawingTechProcessParams SawingTechProcessParams { get; set; }

        //public Tactile.TactileTechProcessParams TactileTechProcessParams { get; set; }
        #endregion
    }
}
