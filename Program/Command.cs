using System;
using System.Collections.Generic;
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

        private sealed class CommandEqualityComparer : IEqualityComparer<Command>
        {
            public bool Equals(Command x, Command y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Number == y.Number && x.Duration.Equals(y.Duration) && x.Text == y.Text && x.ToolPosition.Equals(y.ToolPosition);
            }

            public int GetHashCode(Command obj)
            {
                unchecked
                {
                    var hashCode = obj.Number;
                    hashCode = (hashCode * 397) ^ obj.Duration.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.Text.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.ToolPosition.GetHashCode();
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<Command> Comparer { get; } = new CommandEqualityComparer();
    }
}