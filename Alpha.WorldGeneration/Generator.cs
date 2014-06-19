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
        readonly List<Vector2I> _points = new List<Vector2I>();
        readonly List<Triangle> _triangles = new List<Triangle>();
        private Int32 _width;
        private Int32 _height;

        public void Create(int width, int height, int pointCount, int margin)
        {
            _width = width;
            _height = height;
            while (_points.Count < pointCount)
            {
                Vector2I point = new Vector2I(RandomGenerator.Get(margin, width-margin-1), RandomGenerator.Get(margin, height-margin-1));
                if(!_points.Any(p => Vector2I.Distance(p, point) < margin ))
                    _points.Add(point);
            }
            Vector2I leftBottom = new Vector2I(0, 0);
            Vector2I rightBottom = new Vector2I(width -1, 0);
            Vector2I topLeft = new Vector2I(0, height-1);
            Vector2I topRight = new Vector2I(width -1, height -1);

            //Edge left = new Edge(new Vector2I(0, 0), new Vector2I(0, height - 1));
            //Edge right = new Edge(new Vector2I(width - 1, 0), new Vector2I(width - 1, height - 1));
            //Edge top = new Edge(new Vector2I(0, height - 1), new Vector2I(width - 1, height - 1));
            //Edge bottom = new Edge(new Vector2I(0, 0), new Vector2I(width - 1, 0));
            //Edge diagonal = new Edge(new Vector2I(0, 0), new Vector2I(width - 1, height - 1));

            _triangles.Add(new Triangle(leftBottom, topLeft, topRight));
            _triangles.Add(new Triangle(topRight, rightBottom, leftBottom));

            Draw("points");

            for (int i = _points.Count-1; i >= 0; i--)
            {
                Triangle triangle;
                try
                {
                    triangle = _triangles.First(t => t.Contains(_points[i]));
                }
                catch (Exception e)
                {
                    _points.RemoveAt(i);
                    continue;
                }
                _triangles.AddRange(triangle.Split(_points[i]));
                _triangles.Remove(triangle);
            }
            Draw("triangles");
        }

        private void Draw(string fileName)
        {
            Bitmap bitmap = new Bitmap(_width, _height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Black);
            foreach (Triangle triangle in _triangles)
                graphics.DrawPolygon(new Pen(Color.BlueViolet), new[]
                {
                    new Point(triangle.Points[0].X, triangle.Points[0].Y),
                    new Point(triangle.Points[1].X, triangle.Points[1].Y),
                    new Point(triangle.Points[2].X, triangle.Points[2].Y)
                });
            foreach (Vector2I point in _points)
                bitmap.SetPixel(point.X, point.Y, Color.Crimson);
            bitmap.Save("C:\\Users\\Nicolas\\Desktop\\Voronoi\\"+fileName+".png");
        }
    }
}
