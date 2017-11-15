namespace CAM.Domain
{
    /// <summary>
    /// Управляющая команда технологического процесса обработки
    /// </summary>
    public class TechProcessCommand
    {
        public string Name { get; }

        public string GroupName { get; }

        public TechProcessCommand(string name, string groupName = null)
        {
            Name = name;
            GroupName = groupName;
        }
    }
}