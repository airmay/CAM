namespace CAM.Core
{
    public interface IProcessing : IProcessItem
    {
        IOperation[] Operations { get; set; }
        MachineType MachineType { get; set; }
        void Execute();
        void RemoveAcadObjects();
        void Init();
    }
}
