using System.ComponentModel;

namespace CAM
{
    public enum TechProcessType
    {
        [Description("Распиловка")]
        Sawing,

        [Description("Тактилка")]
        Tactile,

        [Description("Диск 3D")]
        Disk3D,

        [Description("Профиль по сечению")]
        SectionProfile,

        [Description("Сверление")]
        Drilling,

        [Description("Полировка")]
        Polishing,

    }
}
