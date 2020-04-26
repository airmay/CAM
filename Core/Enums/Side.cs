namespace CAM
{
	public enum Side
	{
		None,
		Left,
		Right
	}

    public static class SideExt
    {
        public static Side Opposite(this Side side)
        {
            switch (side)
            {
                case Side.Left:
                    return Side.Right;
                case Side.Right:
                    return Side.Left;
                default:
                    return Side.None;
            }
        }
    }
}
