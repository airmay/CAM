using System;
using System.Collections.Generic;
using CAM.Core.Processing;

namespace CAM.Core;

[Serializable]
public class CamData
{
    public ITechProcess[] TechProcesses { get; set; }
    public List<Command> Commands { get; set; }
    public int? Index { get; set; }
}
