namespace CAM.Core
{
    public interface IOperation
    {
        string Caption { get; set; }
        bool Enabled { get; set; }
    }
}