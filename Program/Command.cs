using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.Core;

namespace CAM
{
    [Serializable]
    public struct Command
    {
        public int Number { get; set; }
        public TimeSpan Duration { get; set; }
        public string Text { get; set; }
        public short OperationNumber { get; set; }
        public ToolPosition ToolPosition { get; set; }

        [NonSerialized] public ObjectId? ObjectId;
        [NonSerialized] public ObjectId? ObjectId2;

        private sealed class CommandEqualityComparer : IEqualityComparer<Command>
        {
            public bool Equals(Command x, Command y)
            {
                return x.Number == y.Number && x.Duration.Equals(y.Duration) && x.Text == y.Text && x.ToolPosition.Equals(y.ToolPosition) && Nullable.Equals(x.ObjectId2, y.ObjectId2);
            }

            public int GetHashCode(Command obj)
            {
                unchecked
                {
                    var hashCode = obj.Number;
                    hashCode = (hashCode * 397) ^ obj.Duration.GetHashCode();
                    hashCode = (hashCode * 397) ^ (obj.Text != null ? obj.Text.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.ToolPosition.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.ObjectId2.GetHashCode();
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<Command> Comparer { get; } = new CommandEqualityComparer();
    }
}