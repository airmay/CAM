using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.Core;

namespace CAM
{
    [Serializable]
    public readonly struct Command(
        int number,
        string text,
        ToolPosition toolPosition,
        TimeSpan duration,
        ObjectId? objectId,
        ObjectId? objectId2,
        short operationNumber)
    {
        public int Number { get; } = number;
        public TimeSpan Duration { get; } = duration;
        public string Text { get; } = text;
        public short OperationNumber { get; } = operationNumber;
        public ToolPosition ToolPosition { get; } = toolPosition;

        [NonSerialized] public readonly ObjectId? ObjectId = objectId;
        [NonSerialized] public readonly ObjectId? ObjectId2 = objectId2;

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