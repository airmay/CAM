using System;

namespace CAM
{
    [Serializable]
    public abstract class WireSawingTechOperation<T> : WireSawingTechOperation where T : ITechProcess
    {
        public T TechProcess => (T)TechProcessBase;
    }

    [Serializable]
    public abstract class WireSawingTechOperation : TechOperation
    {
    }
}
