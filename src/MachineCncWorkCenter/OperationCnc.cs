using System;
using CAM.Core;

namespace CAM.MachineCncWorkCenter;

[Serializable]
public abstract class OperationCnc : OperationBase<TechProcessCnc, ProcessorCnc>;