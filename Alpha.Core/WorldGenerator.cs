using Alpha.Common;

namespace Alpha.Core
{
    public class WorldGenerator : IWorldGenerator
    {
        IProcessableWorld IWorldGenerator.Generate()
        {
            return new World();
        }
    }
}
