using System.Collections.Generic;

namespace CAM
{
    static class ListExtensions
    {
        public static bool Swap<T>(this List<T> list, int firstIndex, int secondIndex)
        {
	        if (!(firstIndex >= 0 && firstIndex < list.Count && secondIndex >= 0 && secondIndex < list.Count && firstIndex != secondIndex))
		        return false;
	        var temp = list[firstIndex];
	        list[firstIndex] = list[secondIndex];
	        list[secondIndex] = temp;
	        return true;
        }

        public static bool SwapNext<T>(this List<T> list, int index) => list.Swap(index, index + 1);

        public static bool SwapPrev<T>(this List<T> list, int index) => list.Swap(index, index - 1);

        public static bool SwapNext<T>(this List<T> list, T item) => SwapNext(list, list.IndexOf(item));

        public static bool SwapPrev<T>(this List<T> list, T item) => SwapPrev(list, list.IndexOf(item));
    }
}
