using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;

namespace Alpha.WorldGeneration
{
    class Generator
    {
        private List<Vector2I> _points;
        private List<Triangle> _triangles;
        private Int32 _width;
        private Int32 _height;

        public void Create(int width, int height, int pointCount, int margin)
        {
            _width = width;
            _height = height;
            _triangles = new List<Triangle>();
            _points = new List<Vector2I>();
            while (_points.Count < pointCount)
            {
                Vector2I point = new Vector2I(RandomGenerator.Get(margin, width-margin-1), RandomGenerator.Get(margin, height-margin-1));
                if(!_points.Any(p => Vector2I.Distance(p, point) < margin ))
                    _points.Add(point);
            }

            //Set the base two triangles
            Vector2I leftBottomPoint = new Vector2I(0, 0);
            Vector2I rightBottomPoint = new Vector2I(width -1, 0);
            Vector2I topLeftPoint = new Vector2I(0, height-1);
            Vector2I topRightPoint = new Vector2I(width -1, height -1);

            Triangle topTriangle, bottomTriangle;
            _triangles.Add(topTriangle = new Triangle(leftBottomPoint, topLeftPoint, topRightPoint));
            _triangles.Add(bottomTriangle = new Triangle(topRightPoint, rightBottomPoint, leftBottomPoint));
            Edge leftEdge = new Edge(topTriangle, leftBottomPoint, topLeftPoint);
            Edge topEdge = new Edge(topTriangle, topLeftPoint, topRightPoint);
            Edge rightEdge = new Edge(bottomTriangle, topRightPoint, rightBottomPoint);
            Edge bottomEdge = new Edge(bottomTriangle, leftBottomPoint, rightBottomPoint);
            Edge diagonalEdge = new Edge(topTriangle, bottomTriangle);
            topTriangle.Edges = new [] {leftEdge, topEdge, diagonalEdge};
            bottomTriangle.Edges = new[] {rightEdge, bottomEdge, diagonalEdge};

            Draw("points", _triangles, _points);

            for (int i = _points.Count-1; i >= 0; i--)
            {
                // Find the triangle containing the new point
                Triangle triangle;
                try
                {
                    triangle = _triangles.First(t => t.Contains(_points[i]));
                }
                catch (Exception e) //Special situation : the point is on the Edge of a triangle or three close points are aligned (very rare case)
                {
                    _points.RemoveAt(i);
                    continue;
                }
                List<Triangle> tempList = new List<Triangle>();
                //Create three new triangles
                for (int j = 0; j < 3; j++)
                    tempList.Add(new Triangle(triangle.Points[j], triangle.Points[(j + 1) % 3], _points[i]));
                //Replace link in the exterior edges to the new triangles
                for (int j = 0; j < 3; j++)
                    triangle.Edges[j].Replace(triangle, tempList[j]);
                //Create interior edges
                List<Edge> interiorEdges = new List<Edge>();
                for (int j = 0; j < 3; j++)
                    interiorEdges.Add(new Edge(tempList[j], tempList[(j + 1) % 3]));
                //Link edges to new triangles
                for (int j = 0; j < 3; j++)
                    tempList[j].Edges = new[] { triangle.Edges[j], interiorEdges[j], interiorEdges[(j + 2) % 3]};
                _triangles.AddRange(tempList);
                _triangles.Remove(triangle);

                //Create local delaunay triangulation
                Stack<Edge> edgesToSolve = new Stack<Edge>();

                //foreach (Edge e in from t in _triangles from e in t.Edges where !edgesToSolve.Contains(e) select e)
                //    edgesToSolve.Push(e);
                foreach (Edge edge in triangle.Edges)
                    edgesToSolve.Push(edge);
                ProcessLocalDelaunay(edgesToSolve);
            }
            Draw("triangles", _triangles, _points);
            //Clean list of triangles using any of the corner points
            Vector2I[] extremePoints = { topLeftPoint, topRightPoint, leftBottomPoint, rightBottomPoint };
            Draw("cleaned", _triangles.Where(t => !t.Points.Any(extremePoints.Contains)).ToList(), _points);
        }

        private void ProcessLocalDelaunay(Stack<Edge> edgesToSolve)
        {
            while (edgesToSolve.Count > 0)
            {
                Edge edge = edgesToSolve.Pop();
                if (edge.Triangles[0] == null || edge.Triangles[1] == null)
                    continue;
                //Evaluate if the edge needs flipping
                Triangle t0 = edge.Triangles[0];
                Triangle t1 = edge.Triangles[1];
                Vector2I oppositePoint0 = t1.Points.First((p) => !t0.Points.Contains(p));
                if (!InCircle(t0.Points[0], t0.Points[1], t0.Points[2], oppositePoint0))
                {
                    //Draw("flip_before"+(++counter), edge.Triangles.ToList(), edge.Triangles[0].Points.Union(edge.Triangles[1].Points).ToList());

                    // flip edge :
                    // step 0 : Reorder edge points to be in opposite order of the triangle 0
                    int position = Array.IndexOf(t0.Points, edge.Points[0]);
                    if (t0.Points[(position + 1)%3] == edge.Points[1])
                    {
                        Vector2I point = edge.Points[0];
                        edge.Points[0] = edge.Points[1];
                        edge.Points[1] = point;
                    }
                    // step 1 : Replace in each triangle one of the point on the edge by the point belonging only to the other triangle
                    Vector2I oppositePoint1 = t0.Points.First((p) => !t1.Points.Contains(p));
                    t0.Points[Array.IndexOf(t0.Points, edge.Points[0])] = oppositePoint0;
                    t1.Points[Array.IndexOf(t1.Points, edge.Points[1])] = oppositePoint1;
                    // step 2 : Reassociate the external edges to the correct triangle
                    Edge e0 = t0.Edges[Array.IndexOf(t0.Points, oppositePoint0)];
                    Edge e1 = t1.Edges[Array.IndexOf(t1.Points, oppositePoint1)];
                    e0.Replace(t0, t1);
                    e1.Replace(t1, t0);
                    t0.Edges[Array.IndexOf(t0.Edges, edge)] = e1;
                    t1.Edges[Array.IndexOf(t1.Edges, edge)] = e0;
                    // step 3 : Correct the interior edge
                    t0.Edges[Array.IndexOf(t0.Points, oppositePoint0)] = edge;
                    t1.Edges[Array.IndexOf(t1.Points, oppositePoint1)] = edge;
                    edge.Points = new[] {oppositePoint0, oppositePoint1};
                    // step 4 : Add to the stack the possibly affected edges
                    foreach (Edge e in edge.Triangles[0].Edges.Union(edge.Triangles[1].Edges))
                        if (e != edge && !edgesToSolve.Contains(e))
                            edgesToSolve.Push(e);
                    //Draw("flip_after" + counter, edge.Triangles.ToList(), edge.Triangles[0].Points.Union(edge.Triangles[1].Points).ToList());
                }
            }
        }


        bool InCircle(Vector2I a, Vector2I b, Vector2I c, Vector2I d)
        {
            return (a.X*a.X + a.Y*a.Y) * TriArea(b, c, d) -
            (b.X*b.X + b.Y*b.Y) * TriArea(a, c, d) +
            (c.X*c.X + c.Y*c.Y) * TriArea(a, b, d) -
            (d.X*d.X + d.Y*d.Y) * TriArea(a, b, c) > 0;
        }

        double TriArea(Vector2I a, Vector2I b, Vector2I c)
        {
            return (b.X - a.X)*(c.Y - a.Y) - (b.Y - a.Y)*(c.X - a.X);
        }

        private void Draw(string fileName, List<Triangle> triangles, List<Vector2I> points)
        {
            Bitmap bitmap = new Bitmap(_width, _height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Black);
            foreach (Triangle triangle in triangles)
                graphics.DrawPolygon(new Pen(Color.BlueViolet), new[]
                {
                    new Point(triangle.Points[0].X, triangle.Points[0].Y),
                    new Point(triangle.Points[1].X, triangle.Points[1].Y),
                    new Point(triangle.Points[2].X, triangle.Points[2].Y)
                });
            foreach (Vector2I point in points)
                bitmap.SetPixel(point.X, point.Y, Color.Crimson);
            bitmap.Save("C:\\Users\\Nicolas\\Desktop\\Voronoi\\"+fileName+".png");
        }
    }
}
