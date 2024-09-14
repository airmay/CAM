namespace CAM.Core
{
    public interface IProcessing
    {
        IOperation[] Operations { get; set; }
        MachineType MachineType { get; set; }
        void Execute();
        void RemoveAcadObjects();
        void Init();
        IProgram Program { get; }

    }
}
