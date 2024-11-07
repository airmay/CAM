using System;

namespace CAM
{
    public interface IProcessor : IDisposable
    {
        void Start();
        void Finish();
        IOperation Operation { get; set; }
    }
}