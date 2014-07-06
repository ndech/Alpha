using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alpha.Toolkit;

namespace Alpha.WorldGeneration
{
    class FloodFillWaterGenerator : IWaterGenerator
    {
        public void GenerateWater(List<VoronoiSite> sites)
        {
            Stack<VoronoiSite> processingStack = new Stack<VoronoiSite>();
            foreach (VoronoiSite site in sites)
            {
                if (site.IsOnBorder || RandomGenerator.GetDouble(0, 80) < 1)
                {
                    site.IsWater = true;
                    processingStack.Push(site);
                }
            }
            while (processingStack.Count > 0)
            {
                if (RandomGenerator.GetDouble(0, 1) < 0.60)
                    continue;
                VoronoiSite site = processingStack.Pop();
                double threshold = RandomGenerator.GetDouble(0, 0.65);
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
