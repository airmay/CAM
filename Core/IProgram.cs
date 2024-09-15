using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.Core
{
    public interface IProgram
    {
        void Reset();
        object GetCommands();
        bool TryGetCommandIndex(ObjectId objectId, out int commandIndex);
        void Export();
    }
}