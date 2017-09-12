using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM
{
    static class IListExtensions
    {
        public static void Swap(this IList list, int firstIndex, int secondIndex)
        {
            Contract.Requires(list != null);
            Contract.Requires(firstIndex >= 0 && firstIndex < list.Count);
            Contract.Requires(secondIndex >= 0 && secondIndex < list.Count);
            if (firstIndex == secondIndex)
                return;

            object temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }

        public static void SwapNext(this IList list, object item)
        {
            var index = list.IndexOf(item);
            list.Swap(index, index + 1);
        }

        public static void SwapPrev(this IList list, object item)
        {
            var index = list.IndexOf(item);
            list.Swap(index, index - 1);
        }
    }
}
