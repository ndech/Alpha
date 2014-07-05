using System.Drawing;
using Alpha.Toolkit;

namespace Alpha.WorldGeneration
{
    class Cluster
    {
        public Color Color { get; set; }
        public int Id { get; set; }
        private static int idSeed = 0;

        public Cluster()
        {
            Color = Color.FromArgb(RandomGenerator.Get(0, 255), RandomGenerator.Get(0, 255), RandomGenerator.Get(0, 255));
            Id = ++idSeed;
        }
    }
}
