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
        public static List<VoronoiSite> Create(int width, int height, int pointCount, int relaxations)
        {
            RandomGenerator.ResetSeed(1256);
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
                    Vector newPoint = new Vector(site.Points.Average(p => p[0]),
                       site.Points.Average(p => p[1]));
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
            Stack<VoronoiSite> ToBeProcessedSites;
            while (true)
            {
                ToBeProcessedSites = new Stack<VoronoiSite>();
                VoronoiSite site = sites.FirstOrDefault(s => s.ShoreDistance == VoronoiSite.DefaultShoreDistance && s.Neighbourgs.Any(p => p.IsWater != s.IsWater));
                if (site == null)
                    break;
                site.ShoreDistance = 1;
                ToBeProcessedSites.Push(site);
                Cluster cluster = new Cluster();
                while (ToBeProcessedSites.Count > 0)
                {
                    VoronoiSite item = ToBeProcessedSites.Pop();
                    item.Cluster = cluster;
                    if (item.Neighbourgs.Any(s => item.IsWater != s.IsWater)) item.ShoreDistance = 1;
                    foreach (VoronoiSite neighbourg in item.Neighbourgs)
                    {
                        if ((neighbourg.IsWater == item.IsWater) && neighbourg.ShoreDistance > item.ShoreDistance + 1)
                        {
                            neighbourg.ShoreDistance = item.ShoreDistance + 1;
                            if (!ToBeProcessedSites.Contains(neighbourg))
                                ToBeProcessedSites.Push(neighbourg);
                        }
                    }
                }
            }
            Draw("terrain", width, height, sites, RenderMode.Terrain);
            Draw("clusters", width, height, sites, RenderMode.Cluster);
            Console.WriteLine("Land tiles  : " + sites.Count(s => !s.IsWater) +" ("+ String.Format("{0:P}", (float)sites.Count(s => !s.IsWater) / sites.Count)+")");
            Console.WriteLine("Water tiles : " + sites.Count(s => s.IsWater) + " (" + String.Format("{0:P}", (float)sites.Count(s => s.IsWater) / sites.Count)+")");
            //Console.ReadKey();
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

        private static void Draw(string fileName, int width, int height, List<VoronoiSite> sites, RenderMode mode)
        {
            Bitmap bitmap = new Bitmap(width+1, height+1);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(Color.Black);

            foreach (VoronoiSite site in sites)
            {
                if(site.Points.Count==0)
                    return;
                PointF[] list = new PointF[site.Points.Count];
                for (int i = 0; i < site.Points.Count; i++)
                    list[i] = new PointF((float)site.Points[i][0], (float)site.Points[i][1]);
                if (mode == RenderMode.Terrain)
                {
                    if (site.IsWater)
                        graphics.FillPolygon(new SolidBrush(Color.FromArgb(73, 156 - 5 * site.ShoreDistance, 203)), list);
                    else
                        graphics.FillPolygon(new SolidBrush(Color.FromArgb(73, 210 - 5 * site.ShoreDistance, 49)), list);
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
