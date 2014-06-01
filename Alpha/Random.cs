using System.Collections.Generic;
using System.Linq;
using SystemRandom = System.Random;

namespace Alpha
{
    class Random
    {
        private readonly SystemRandom _generator;
        private Random()
        {
            _generator = new SystemRandom();
        }

        private static Random _instance;

        public int Get(int min, int max)
        {
            return _generator.Next(min, max);
        }

        public double GetDouble(double min, double max)
        {
            return _generator.NextDouble() * max + min;
        }

        public static Random Generator { get { return _instance ?? (_instance = new Random()); } }
    }

    static class RandomIEnumerableExtensions
    {
        public static T RandomItem<T>(this IEnumerable<T> source)
        {
            return source.ElementAt(Random.Generator.Get(0, source.Count()));
        }
    }
}
