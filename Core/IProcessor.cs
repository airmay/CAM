using System;

namespace CAM
{
    public interface IProcessor : IDisposable
    {
        void Start();
        void Finish();
        void SetOperation(OperationBase operation);
    }
}