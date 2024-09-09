namespace CAM.Core
{
    public interface IProcessItem
    {
        string Caption { get; set; }
        bool Enabled { get; set; }
        IProcessItem[] Children { get; set; }
        int CommandIndex { get; set; }
        void Delete();
        void Select();
    }
}