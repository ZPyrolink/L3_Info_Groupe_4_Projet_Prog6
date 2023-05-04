using System.Collections.Generic;
using System.Linq;

namespace Taluva.Utils
{
    public static class EnumerableExt
    {
        public static IEnumerable<T> Order<T>(this IEnumerable<T> source) => source.OrderBy(v => v);
    }
}