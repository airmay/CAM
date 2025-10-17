using System.ComponentModel;

namespace CAM.Core.Tools
{
    public enum ToolType
    {
        [Description("Диск")]
        Disk,

        [Description("Фреза")]
        Mill,

        [Description("Трос")]
        WireSaw
    }
}
