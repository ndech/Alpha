using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Toolkit;
using Alpha.Voronoi;

namespace Alpha.WorldGeneration
{
    public static class Generator
    {
        public static List<VoronoiSite> Create(int width, int height, int pointCount, int relaxations, int seed)
        {
            RandomGenerator.ResetSeed(seed);
            Dictionary<Vector, VoronoiSite> points = new Dictionary<Vector, VoronoiSite>(pointCount);
            for (int i = 0; i < pointCount; i++)
            {
                Vector point = new Vector(RandomGenerator.Get(0, width), RandomGenerator.Get(0, height));
                try
                {
                    points.Add(point, new VoronoiSite(point));
                }
                catch (ArgumentException) // In case two points are to close one from each other
                {
                    i--;
                }
            }
            List<VoronoiSite> sites = CalculateVoronoiGraph(points, width, height);
            for (int i = 0; i < relaxations; i++)
            {
                Dictionary<Vector, VoronoiSite> newPoints = new Dictionary<Vector, VoronoiSite>();
                foreach (VoronoiSite site in sites)
                {
                    Vector newPoint = new Vector(site.VoronoiPoints.Average(p => p[0]),
                       site.VoronoiPoints.Average(p => p[1]));
                    newPoints.Add(newPoint, new VoronoiSite(newPoint));
                }
                points = newPoints;
                sites = CalculateVoronoiGraph(points, width, height);
            }
            //Create water
            IWaterGenerator waterGenerator = new FloodFillWaterGenerator();
            //waterGenerator = new PerlinWaterGenerator();
            waterGenerator.GenerateWater(sites);
            //Associate water distance from shore and split the water in cluster (independant seas)
            while (true)
            {
                Stack<VoronoiSite> toBeProcessedSites = new Stack<VoronoiSite>();
                VoronoiSite site = sites.FirstOrDefault(s => s.ShoreDistance == VoronoiSite.DefaultShoreDistance && s.Neighbourgs.Any(p => p.IsWater != s.IsWater));
                if (site == null)
                    break;
                site.ShoreDistance = 1;
                toBeProcessedSites.Push(site);
                Cluster cluster = new Cluster();
                while (toBeProcessedSites.Count > 0)
                {
                    VoronoiSite item = toBeProcessedSites.Pop();
                    item.Cluster = cluster;
                    if (item.Neighbourgs.Any(s => item.IsWater != s.IsWater)) item.ShoreDistance = 1;
                    foreach (VoronoiSite neighbourg in item.Neighbourgs)
                    {
                        if ((neighbourg.IsWater == item.IsWater) && neighbourg.ShoreDistance > item.ShoreDistance + 1)
                        {
                            neighbourg.ShoreDistance = item.ShoreDistance + 1;
                            if (!toBeProcessedSites.Contains(neighbourg))
                                toBeProcessedSites.Push(neighbourg);
                        }
                    }
                }
            }
            DebugConsole.WriteLine("Land tiles  : " + sites.Count(s => !s.IsWater) +" ("+ string.Format("{0:P}", (float)sites.Count(s => !s.IsWater) / sites.Count)+")");
            DebugConsole.WriteLine("Water tiles : " + sites.Count(s => s.IsWater) + " (" + string.Format("{0:P}", (float)sites.Count(s => s.IsWater) / sites.Count)+")");
            return sites;
        }

        private static List<VoronoiSite> CalculateVoronoiGraph(Dictionary<Vector, VoronoiSite> points, int width, int height)
        {
            VoronoiGraph result = Fortune.ComputeVoronoiGraph(points.Keys.ToList());
            foreach (VoronoiEdge edge in result.Edges)
            {
                VoronoiSite a = points[edge.LeftData];
                VoronoiSite b = points[edge.RightData];
                a.Neighbourgs.Add(b);
                b.Neighbourgs.Add(a);
                a.Edges.Add(edge);
                b.Edges.Add(edge);
            }
            foreach (VoronoiSite site in points.Values)
                site.Reorder(width, height);
            return points.Values.ToList();
        }
    }
}
