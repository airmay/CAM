using System;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.Core;

namespace CAM
{
    public class Command
    {
        public int Number { get; set; }
        public TimeSpan Duration { get; set; }
        public string Text { get; set; }
        public ObjectId? ObjectId { get; set; }
        public ObjectId? ObjectId2 { get; set; }
        public short OperationNumber { get; set; }
        public ToolPosition ToolPosition { get; set; }
    }
}