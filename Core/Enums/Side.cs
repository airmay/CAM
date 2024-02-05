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
    }
}
