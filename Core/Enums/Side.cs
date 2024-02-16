namespace CAM
{
	public enum Side
	{
        Left = -1,
        None = 0,
        Right = 1,
    }

    public static class SideExt
    {
        public static Side Opposite(this Side side) => (Side)(-1 * (int)side);
        public static bool IsLeft(this Side side) => side == Side.Left;
        public static bool IsNone(this Side side) => side == Side.None;
        public static bool IsRight(this Side side) => side == Side.Right;
    }
}
