using System.Collections.Generic;

namespace CAM;

public class CamData
{
    public IProcessing[] TechProcesses { get; set; }
    public List<Command> Commands { get; set; }
    public int? Index { get; set; }
}
