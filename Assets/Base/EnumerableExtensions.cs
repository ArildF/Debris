using System.Collections.Generic;

namespace Base
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> BatchBy<T>(this IEnumerable<T> seq, int partitionSize)
        {
            using (var enumerator = seq.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return enumerator.TakeFromCurrent(partitionSize);
                }
            }
        }
        public static IEnumerable<T> TakeFromCurrent<T>(this IEnumerator<T> enumerator, int count)
        {
            while (count > 0)
            {
                yield return enumerator.Current;
                if (--count > 0 && !enumerator.MoveNext()) yield break;
            }
        }
    }
}