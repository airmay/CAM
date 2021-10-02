using Autodesk.AutoCAD.DatabaseServices;

namespace CAM
{
    public class ProcessCommand
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public bool HasTool { get; set; }

        public ToolPosition ToolLocation;

        public ObjectId? ToolpathObjectId { get; set; }

        public double Duration { get; set; }

        // TODO настройка формата
        public string GetProgrammLine(string formatString) => Text; // string.Format(formatString, Number) + Text;

        public object Owner { get; set; }

    }
}