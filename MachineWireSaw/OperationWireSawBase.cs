using System;
using CAM.Core;

namespace CAM.MachineWireSaw;

[Serializable]
public abstract class OperationWireSawBase : OperationBase<TechProcessWireSaw, ProcessorWireSaw>;