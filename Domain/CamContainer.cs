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
        private static readonly string _filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cam_data.xml");

        #region Methods

        /// <summary>
        /// Загрузить данные их файла в контейнер
        /// </summary>
        /// <returns></returns>
        public static CamContainer Load()
        {
            var formatter = new XmlSerializer(typeof(CamContainer));
            try
            {
                using (var fileStream = new FileStream(_filePath, FileMode.Open))
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
        public void Save()
        {
            var formatter = new XmlSerializer(typeof(CamContainer));
            try
            {
                using (var fileStream = new FileStream(_filePath, FileMode.Create))
                    formatter.Serialize(fileStream, this);
            }
            catch (Exception e)
            {
                Application.ShowAlertDialog($"Ошибка при сохранении данных в файл 'cam_data.xml':\n{e.Message}");
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
