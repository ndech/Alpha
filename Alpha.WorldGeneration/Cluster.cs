using System.Drawing;
using Alpha.Toolkit;

namespace Alpha.WorldGeneration
{
    public class Cluster
    {
        public int Id { get; set; }
        private static int idSeed = 0;

        public Cluster()
        {
            Id = ++idSeed;
        }
    }
}
