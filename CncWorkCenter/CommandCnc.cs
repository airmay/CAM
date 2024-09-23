using Autodesk.AutoCAD.DatabaseServices;
using CAM.Core;

namespace CAM.CncWorkCenter
{
    public class CommandCnc: ICommand
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public ObjectId? ObjectId { get; set; }
        public ToolLocationCnc ToolLocation { get; set; }
        public OperationCnc Operation { get; set; }

        public void ShowTool()
        {
            //ToolObject.Curves
        }
    }
}