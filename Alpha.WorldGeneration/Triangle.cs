using System;
using Alpha.Toolkit.Math;

namespace Alpha.WorldGeneration
{
    class Triangle
    {
        //Contains the tree points of the triangle in a clockwise order
        public readonly Vector2I[] Points = new Vector2I[3];
        public Edge[] Edges = new Edge[3];
        public int Id { get; }
        public Vector2I A { get { return Points[0]; } }
        public Vector2I B { get { return Points[1]; } }
        public Vector2I C { get { return Points[2]; } }

        public Vector2I Circumcenter
        {
            //http://en.wikipedia.org/wiki/Circumcenter#Cartesian_coordinates
            get
            {
                double d = 2 * (A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y) + C.X * (A.Y - B.Y));
                double x = A.SquaredSize() * (B.Y - C.Y) + B.SquaredSize() * (C.Y - A.Y) + C.SquaredSize() * (A.Y - B.Y);
                double y = A.SquaredSize() * (B.X - C.X) + B.SquaredSize() * (C.X - A.X) + C.SquaredSize() * (A.X - B.X);
                return (new Vector2I((int)(x / d), -(int)(y / d)));
            }
        }

        public static int AutoIncrement = 0;
        public Triangle(Vector2I a, Vector2I b, Vector2I c)
        {
            Points[0] = a;
            Points[1] = b;
            Points[2] = c;
            Id = AutoIncrement++;
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

        public override string ToString()
        {
            return ""+Id;
        }
    }
}
