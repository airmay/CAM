using System.Collections;

namespace CAM
{
    static class ListExtensions
    {
        public static bool Swap(this IList list, int firstIndex, int secondIndex)
        {
	        if (!(firstIndex >= 0 && firstIndex < list.Count && secondIndex >= 0 && secondIndex < list.Count && firstIndex != secondIndex))
		        return false;
	        (list[firstIndex], list[secondIndex]) = (list[secondIndex], list[firstIndex]);
            return true;
        }

        public static bool SwapNext(this IList list, int index) => list.Swap(index, index + 1);

        public static bool SwapPrev(this IList list, int index) => list.Swap(index, index - 1);

        public static bool SwapNext(this IList list, object item) => SwapNext(list, list.IndexOf(item));

        public static bool SwapPrev(this IList list, object item) => SwapPrev(list, list.IndexOf(item));
    }
}
