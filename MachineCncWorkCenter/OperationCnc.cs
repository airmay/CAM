using System;
using CAM.CncWorkCenter;

namespace CAM;

[Serializable]
public abstract class OperationCnc : OperationBase<ProcessingCnc, ProcessorCnc>;