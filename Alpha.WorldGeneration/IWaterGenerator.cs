using System.Collections.Generic;

namespace Alpha.WorldGeneration
{
    interface IWaterGenerator
    {
        void GenerateWater(List<VoronoiSite> sites);
    }
}
