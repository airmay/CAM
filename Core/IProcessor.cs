using System;

namespace CAM
{
    public interface IProcessor
    {
        void Start();
        void Finish();
        void SetOperation(IOperation operation);
    }
}