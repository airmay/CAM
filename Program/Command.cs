using System;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.Core;

namespace CAM
{
    [Serializable]
    public class Command
    {
        public int Number { get; set; }
        public TimeSpan Duration { get; set; }
        public string Text { get; set; }
        public short OperationNumber { get; set; }
        public ToolPosition ToolPosition { get; set; }

        [NonSerialized] public ObjectId? ObjectId;
        [NonSerialized] public ObjectId? ObjectId2;
    }
}