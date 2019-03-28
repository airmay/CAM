using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CAM.Domain
{
    /// <summary>
    /// Хранилище персистентных данных 
    /// </summary>
    public class CamContainer
    {
        private static string _filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cam_data.json");

        #region Methods

        /// <summary>
        /// Загрузить данные их файла в контейнер
        /// </summary>
        /// <returns></returns>
        public static CamContainer Load()
        {
            try
            {
                return JsonSerialization.ReadFromJsonFile<CamContainer>(_filePath);
            }
            catch (Exception)
            {
                // TODO Message
                return new CamContainer
                {
                    Tools = new List<Tool>(),
                    SawingLineTechOperationParams = new SawingTechOperationParams(),
                    SawingCurveTechOperationParams = new SawingTechOperationParams()
                };
            }
        }

        /// <summary>
        /// Сохранить данные в файл
        /// </summary>
        public void Save() => JsonSerialization.WriteToJsonFile(_filePath, this); 

        #endregion


        public List<Tool> Tools { get; set; }

        #region TechOperationParams

        public SawingTechOperationParams SawingLineTechOperationParams { get; set; }

        public SawingTechOperationParams SawingCurveTechOperationParams { get; set; } 

        #endregion
    }
}
