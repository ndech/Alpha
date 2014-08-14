using System.Collections.Generic;
using System.Linq;
using Alpha.Toolkit;

namespace Alpha.WorldGeneration
{
    class PerlinWaterGenerator : IWaterGenerator
    {
        public void GenerateWater(List<VoronoiSite> sites)
        {
            NoiseGenerator.Frequency = 0.0005;
            foreach (VoronoiSite site in sites)
            {
                if ((site.IsOnBorder && RandomGenerator.GetDouble(0, 1) < 0.60) || (NoiseGenerator.Noise((int) site.VoronoiCenter[0], (int) site.VoronoiCenter[1]) + 1)*127 < 125)
                    site.IsWater = true;
            }
            Stack<VoronoiSite> processingStack = new Stack<VoronoiSite>(sites.Where(s=>s.IsWater));
            while (processingStack.Count > 0)
            {
                VoronoiSite site = processingStack.Pop();
                if (!site.IsOnBorder && RandomGenerator.GetDouble(0, 1) < 0.50)
                    continue;
                double threshold = site.IsOnBorder ? RandomGenerator.GetDouble(0, 0.85) : RandomGenerator.GetDouble(0, 0.67);
                foreach (VoronoiSite target in site.Neighbourgs)
                {
                    if (target.IsWater)
                        continue;
                    if (RandomGenerator.GetDouble(0, 1) < threshold)
                    {
                        target.IsWater = true;
                        if (!processingStack.Contains(target))
                            processingStack.Push(target);
                    }
                }
            }
            //Remove single cell islands
            foreach (VoronoiSite island in sites.Where(p => !p.IsWater && p.Neighbourgs.All(i => i.IsWater)))
                island.IsWater = true;
        }
    }
}
