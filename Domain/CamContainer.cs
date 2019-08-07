using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace CAM.Domain
{
    /// <summary>
    /// Хранилище персистентных данных 
    /// </summary>
    public class CamContainer
    {
        private static CamContainer _instance;

        public static CamContainer Instance => _instance ?? (_instance = LoadData());

        private static string GetFilePath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cam_data.xml");

        #region static methods

        /// <summary>
        /// Загрузить данные из файла в контейнер
        /// </summary>
        /// <returns></returns>
        public static CamContainer LoadData()
        {
            var formatter = new XmlSerializer(typeof(CamContainer));
            try
            {
                using (var fileStream = new FileStream(GetFilePath, FileMode.Open))
                    return (CamContainer)formatter.Deserialize(fileStream);
            }
            catch (Exception e)
            {
                Application.ShowAlertDialog($"Ошибка при загрузке данных из файла 'cam_data.xml' :\n{e.Message}");
                return new CamContainer
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
        public static void SaveData()
        {
            if (_instance == null) return;

            var formatter = new XmlSerializer(typeof(CamContainer));
            try
            {
                using (var fileStream = new FileStream(GetFilePath, FileMode.Create))
                    formatter.Serialize(fileStream, _instance);
            }
            catch (Exception e)
            {
                Acad.Alert($"Ошибка при сохранении данных в файл 'cam_data.xml':\n{e.Message}");
            }
        }

        #endregion

        public List<Tool> Tools { get; set; }

        public TechProcessParams TechProcessParams { get; set; }

        #region TechOperationParams

        public SawingTechOperationParams SawingLineTechOperationParams { get; set; }

        public SawingTechOperationParams SawingCurveTechOperationParams { get; set; } 

        #endregion
    }
}
