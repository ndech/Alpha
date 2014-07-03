using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Alpha.Voronoi;
using Alpha.Toolkit;

namespace Alpha.WorldGeneration
{
    public static class Generator
    {
        public static void Create(int width, int height, int pointCount, int relaxations)
        {
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
            foreach (VoronoiSite site in points.Values.Where(s => s.IsOnBorder))
            {
                site.IsWater = true;
                processingStack.Push(site);
            }
            while (processingStack.Count > 0)
            {
                VoronoiSite site = processingStack.Pop();
                double threshold = RandomGenerator.GetDouble(0, 0.7);
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
            Draw("voronoi", width, height, points);
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

        private static void Draw(string fileName, int width, int height, Dictionary<Vector, VoronoiSite> points)
        {
            Bitmap bitmap = new Bitmap(width+1, height+1);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Black);

            foreach (VoronoiSite site in points.Values)
            {
                PointF[] list = new PointF[site.Points.Count];
                for (int i = 0; i < site.Points.Count; i++)
                    list[i] = new PointF((float)site.Points[i][0], (float)site.Points[i][1]);
                try
                {
                    if(site.IsWater)
                        graphics.FillPolygon(new SolidBrush(Color.Blue), list);
                    else
                        graphics.FillPolygon(new SolidBrush(Color.DarkGray), list);
                    graphics.DrawPolygon(new Pen(Color.DarkRed), list);
                }
                catch (Exception) { }
            }
            foreach (VoronoiSite site in points.Values)
                bitmap.SetPixel((int)site.Center[0], (int)site.Center[1], Color.Crimson);
            bitmap.Save("C:\\Users\\Nicolas\\Desktop\\Voronoi\\"+fileName+".png");
        }
    }
}
