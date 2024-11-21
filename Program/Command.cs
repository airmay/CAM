using Autodesk.AutoCAD.DatabaseServices;
using CAM.Core;

namespace CAM
{
    public class Command
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public ObjectId? ObjectId { get; set; }
        public IOperation Operation { get; set; }
        public ToolLocationParams? ToolLocationParams { get; set; }
    }
}