using System.Collections.Generic;
using System.Linq;

namespace Alpha.Toolkit
{
    public class RandomGenerator
    {
        private static readonly System.Random Generator = new System.Random();
        
        public static int Get(int min, int max)
        {
            return Generator.Next(min, max);
        }

        public static double GetDouble(double min, double max)
        {
            return Generator.NextDouble() * max + min;
        }
    }

    public static class RandomIEnumerableExtensions
    {
        public static T RandomItem<T>(this IEnumerable<T> source)
        {
            return source.ElementAt(RandomGenerator.Get(0, source.Count()));
        }
    }
}
