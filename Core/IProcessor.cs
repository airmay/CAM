using System;

namespace CAM
{
    public interface IProcessor : IDisposable
    {
        void Start();
        void Finish();
        OperationBase Operation { get; set; }
    }
}