namespace CAM
{
	/// <summary>
	/// Край 
	/// </summary>
    public enum Corner
    {
        Start,
        End
    }

	public static class CornerExt
	{
		public static Corner Swap(this Corner corner) => (Corner) (1 - (int) corner);
	}

}