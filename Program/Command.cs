using Autodesk.AutoCAD.DatabaseServices;

namespace CAM
{
    public class Command
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public ObjectId? ObjectId { get; set; }
        public OperationBase OperationBase { get; set; }
        public IToolLocation ToolLocation { get; set; }
    }
}