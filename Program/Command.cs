using Autodesk.AutoCAD.DatabaseServices;

namespace CAM
{
    public class Command
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public bool HasTool { get; set; }

        public ToolPosition ToolLocation;

        public ObjectId? ToolpathObjectId { get; set; }

        public double Duration { get; set; }

        // TODO настройка формата
        public string GetProgrammLine(string formatString) => string.Format(formatString, Number) + Text;

        public Operation Operation { get; set; }

        public double? U { get; set; }
        public double? V { get; set; }
        public double? A { get; set; }

    }
}