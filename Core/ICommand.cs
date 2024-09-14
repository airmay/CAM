using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.Core
{
    public interface ICommand
    {
        int Number { get; set; }
        string Name { get; set; }
        string Text { get; set; }
        ObjectId? ObjectId { get; set; }
    }
}