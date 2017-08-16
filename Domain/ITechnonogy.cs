namespace CAM
{
    /// <summary>
    /// Технология обработки изделия
    /// </summary>
    public interface ITechnonogy
    {
        /// <summary>
        /// Создает технологический процесс 
        /// </summary>
        /// <returns></returns>
        TechProcess CreateTechProcess();
    }
}