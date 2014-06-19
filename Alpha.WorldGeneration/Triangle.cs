using System.Collections.Generic;
using Alpha.Toolkit.Math;

namespace Alpha.WorldGeneration
{
    class Triangle
    {
        //Contains the tree points of the triangle in a clockwise order
        public readonly Vector2I[] Points = new Vector2I[3];

        public Triangle(Vector2I a, Vector2I b, Vector2I c)
        {
            Points[0] = a;
            Points[1] = b;
            Points[2] = c;
        }

        public bool Contains(Vector2I point)
        {
            bool b1 = Sign(point, Points[0], Points[1]) < 0.0f;
            bool b2 = Sign(point, Points[1], Points[2]) < 0.0f;
            bool b3 = Sign(point, Points[2], Points[0]) < 0.0f;
            return ((b1 == b2) && (b2 == b3));
        }
        int Sign(Vector2I p1, Vector2I p2, Vector2I p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        public IEnumerable<Triangle> Split(Vector2I point)
        {
            IList<Triangle> list = new List<Triangle>();
            for (int i = 0; i < 3; i++)
                list.Add(new Triangle(Points[i], Points[(i + 1) % 3], point));
            return list;
        }
    }
}
