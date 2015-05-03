using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace Alpha.Toolkit
{
    public static class LinqExtension
    {
        public static void ThrowIfNull<T>(this T obj, string parameterName) where T : class
        {
            if (obj == null) throw new ArgumentNullException(parameterName);
        }

        public static IEnumerable<T> Union<T>(this IEnumerable<T> source, T item)
        {
            return source.Union(Enumerable.Repeat(item, 1));
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T item)
        {
            return source.Except(Enumerable.Repeat(item, 1));
        }

        public static IEnumerable<T> OrderByRandom<T>(this IEnumerable<T> source)
        {
            var buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = RandomGenerator.Get(i, buffer.Count);
                yield return buffer[j];
                buffer[j] = buffer[i];
            }
        }
        
        public static T RandomWeightedItem<T>(this IEnumerable<T> items, Func<T, double> weight)
        {
            List<Tuple<double, T>> tuples = items.Select(i => new Tuple<double, T>(weight(i),i)).ToList();
            double cumulativeProbability = tuples.Sum(r => r.Item1);
            double position = RandomGenerator.GetDouble(0, cumulativeProbability);
            Tuple<double, T> value = null;
            double cursor = 0;
            foreach (Tuple<double, T> tuple in tuples)
            {
                if (position >= cursor && position <= cursor + tuple.Item1)
                {
                    value = tuple;
                    break;
                }
                cursor += tuple.Item1;
            }
            return (value ?? tuples.Last()).Item2;
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
    Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, Comparer<TKey>.Default);
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            source.ThrowIfNull("source");
            selector.ThrowIfNull("selector");
            comparer.ThrowIfNull("comparer");
            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence was empty");
                }
                TSource min = sourceIterator.Current;
                TKey minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }

        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        public static IEnumerable<T> Times<T>(this IEnumerable<T> items, int repeat)
        {
            for (int i = 0; i < repeat; i++)
                foreach (T item in items)
                    yield return item;
        }

        public static IEnumerable<T> Jump<T>(this IEnumerable<T> data, int step, int? count = null)
        {
            IList<T> list = data.ToList();
            count = count ?? list.Count;
            for(int i = 0; i< count; i++)
                yield return list[(step+i)%list.Count];
        }

        public static Vector3 AverageVector(this IEnumerable<Vector3> source)
        {
            List<Vector3> values = source.ToList();
            Vector3 result = new Vector3();
            foreach (Vector3 value in values)
            {
                result += value;
            }
            return result/values.Count;
        }
    }
}
