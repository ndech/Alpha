using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Policy;
using Alpha.Voronoi;
using Alpha.Toolkit;

namespace Alpha.WorldGeneration
{
    enum RenderMode
    {
        Terrain,
        Cluster
    }
    public static class Generator
    {
        public static void Create(int width, int height, int pointCount, int relaxations)
        {
            //RandomGenerator.ResetSeed(1236);
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
            CalculateVoronoiGraph(points, width, height);
            for (int i = 0; i < relaxations; i++)
            {
                Dictionary<Vector, VoronoiSite> newPoints = new Dictionary<Vector, VoronoiSite>();
                foreach (VoronoiSite site in points.Values)
                {
                    Vector newPoint = new Vector(site.Points.Average(p => p[0]),
                       site.Points.Average(p => p[1]));
                    newPoints.Add(newPoint, new VoronoiSite(newPoint));
                }
                points = newPoints;
                CalculateVoronoiGraph(points, width, height);
            }
            //Create water
            Stack<VoronoiSite> processingStack = new Stack<VoronoiSite>();
            foreach (VoronoiSite site in points.Values)
            {
                if (site.IsOnBorder || RandomGenerator.GetDouble(0, 80) < 1)
                {
                    site.IsWater = true;
                    processingStack.Push(site);
                }
            }
            while (processingStack.Count > 0)
            {
                if(RandomGenerator.GetDouble(0,1)<0.80)
                    continue;
                VoronoiSite site = processingStack.Pop();
                double threshold = RandomGenerator.GetDouble(0, 0.55);
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
            foreach (VoronoiSite island in points.Values.Where(p=>!p.IsWater && p.Neighbourgs.All(i => i.IsWater)))
                island.IsWater = true;
            //Associate water distance from shore and split the water in cluster (independant seas)
            Stack<VoronoiSite> ToBeProcessedSites;
            while (true)
            {
                ToBeProcessedSites = new Stack<VoronoiSite>();
                VoronoiSite site = points.Values.FirstOrDefault(s => s.IsWater && s.BaseWaterDepth == VoronoiSite.DefaultBaseHeight && s.Neighbourgs.Any(p => !p.IsWater));
                if (site == null)
                    break;
                site.BaseWaterDepth = 1;
                ToBeProcessedSites.Push(site);
                Cluster cluster = new Cluster();
                while (ToBeProcessedSites.Count > 0)
                {
                    VoronoiSite item = ToBeProcessedSites.Pop();
                    item.Cluster = cluster;
                    if (item.Neighbourgs.Any(s => !s.IsWater)) item.BaseWaterDepth = 1;
                    foreach (VoronoiSite neighbourg in item.Neighbourgs)
                    {
                        if (neighbourg.IsWater && neighbourg.BaseWaterDepth > item.BaseWaterDepth + 1)
                        {
                            neighbourg.BaseWaterDepth = item.BaseWaterDepth + 1;
                            if (!ToBeProcessedSites.Contains(neighbourg))
                                ToBeProcessedSites.Push(neighbourg);
                        }
                    }
                }
            }
            Draw("terrain", width, height, points, RenderMode.Terrain);
            Draw("clusters", width, height, points, RenderMode.Cluster);
        }

        private static void CalculateVoronoiGraph(Dictionary<Vector, VoronoiSite> points, int width, int height)
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
        }

        private static void Draw(string fileName, int width, int height, Dictionary<Vector, VoronoiSite> points, RenderMode mode)
        {
            Bitmap bitmap = new Bitmap(width+1, height+1);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(Color.Black);

            foreach (VoronoiSite site in points.Values)
            {
                PointF[] list = new PointF[site.Points.Count];
                for (int i = 0; i < site.Points.Count; i++)
                    list[i] = new PointF((float)site.Points[i][0], (float)site.Points[i][1]);
                if (mode == RenderMode.Terrain)
                {
                    if (site.IsWater)
                    {
                        if (site.BaseWaterDepth != VoronoiSite.DefaultBaseHeight)
                            graphics.FillPolygon(new SolidBrush(Color.FromArgb(73, 156 - 10 * site.BaseWaterDepth, 203)), list);
                    }
                    else
                        graphics.FillPolygon(new SolidBrush(Color.FromArgb(73, 210 - 10 * (site.BaseLandHeight==VoronoiSite.DefaultBaseHeight ? 0 : site.BaseLandHeight), 49)), list);
                }
                else if (mode == RenderMode.Cluster)
                {
                    if (site.Cluster != null)
                        graphics.FillPolygon(new SolidBrush(site.Cluster.Color), list);
                    else
                        graphics.FillPolygon(new SolidBrush(Color.DarkGray), list);
                }
                graphics.DrawPolygon(new Pen(Color.Black), list);
            }
            bitmap.Save("C:\\Users\\Nicolas\\Desktop\\Voronoi\\"+fileName+".png");
        }
    }
}
