using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chivalry.Utils
{
    public static class Utils
    {
        // copied from https://gist.github.com/2463179
        public static IEnumerable<Tuple<T, T>> Pairwise<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var previous = default(T);

            using (var enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    previous = enumerator.Current;
                }

                while (enumerator.MoveNext())
                {
                    yield return Tuple.Create(previous, enumerator.Current);
                    previous = enumerator.Current;
                }
            }
        }
    }
}
