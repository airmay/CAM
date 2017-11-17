using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace CAM
{
    static class IListExtensions
    {
        public static void Swap<T>(this List<T> list, int firstIndex, int secondIndex)
        {
            Contract.Requires(list != null);
            Contract.Requires(firstIndex >= 0 && firstIndex < list.Count);
            Contract.Requires(secondIndex >= 0 && secondIndex < list.Count);
            if (firstIndex == secondIndex)
                return;

            T temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }

        public static void SwapNext<T>(this List<T> list, T item)
        {
            var index = list.IndexOf(item);
            list.Swap(index, index + 1);
        }

        public static void SwapPrev<T>(this List<T> list, T item)
        {
            var index = list.IndexOf(item);
            list.Swap(index, index - 1);
        }
    }
}
