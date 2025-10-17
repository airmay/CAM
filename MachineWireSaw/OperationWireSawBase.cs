using System;
using CAM.CncWorkCenter;
using CAM.MachineWireSaw;

namespace CAM;

[Serializable]
public abstract class OperationWireSawBase : OperationBase<TechProcessWireSaw, ProcessorWireSaw>;