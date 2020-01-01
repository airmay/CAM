using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
	public enum Side
	{
		None,
		Left,
		Right
	}

    public static class SideExt
    {
        public static Side Swap(this Side side)
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
