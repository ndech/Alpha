using System;
using System.Collections.Generic;
using System.Linq;

namespace Alpha.Toolkit
{
    public static class RandomGenerator
    {
        private static Random _generator = new Random();
        
        public static int Get(int min, int max)
        {
            return _generator.Next(min, max);
        }

        public static double GetDouble(double min, double max)
        {
            return _generator.NextDouble() * max + min;
        }

        public static void ResetSeed(int seed)
        {
            _generator = new Random(seed);
        }
    }

    public static class RandomIEnumerableExtensions
    {
        public static T RandomItem<T>(this IEnumerable<T> source)
        {
            int count = source.Count();
            int randomPosition = RandomGenerator.Get(0, count);
            return count == 0 ? default(T) : source.ElementAt(randomPosition);
        }
    }
}
