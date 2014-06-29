using System;
using System.Linq;
using Alpha.Toolkit.Math;

namespace Alpha.WorldGeneration
{
    class Edge
    {
        public Vector2I[] Points = new Vector2I[2];
        public readonly Triangle[] Triangles = new Triangle[2];

        public Edge(Triangle a, Triangle b)
        {
            Triangles[0] = a;
            Triangles[1] = b;
            int cursor = 0;
            foreach (Vector2I point in a.Points)
            {
                foreach (Vector2I point2 in b.Points)
                {
                    if (point == point2)
                    {
                        if(cursor>=2)
                            throw new InvalidOperationException("Too much common point between the two triangles used to construct the edge");
                        Points[cursor] = point;
                        cursor++;
                    }
                }
            }
            if (cursor != 2)
                throw new InvalidOperationException("Too few common point between the two triangles used to construct the edge");
        }

        public Edge(Triangle onlyOne, Vector2I a, Vector2I b)
        {
            if(!(onlyOne.Points.Contains(a) && onlyOne.Points.Contains(b)))
                throw new InvalidOperationException("Points do not belong to the triangle.");
            Points[0] = a;
            Points[1] = b;
            Triangles[0] = null;
            Triangles[1] = onlyOne;
        }

        public void Replace(Triangle oldTriangle, Triangle newTriangle)
        {
            for (int i = 0; i < 2; i++)
                if (Triangles[i] == oldTriangle)
                {
                    Triangles[i] = newTriangle;
                    if(Triangles[0]==Triangles[1])
                        throw new InvalidOperationException("An edge can not refer twice to the same triangle.");
                    return;
                }
            throw new InvalidOperationException("No matching triangle.");
        }

        public override string ToString()
        {
            return (Triangles[0] == null ? "x" : Triangles[0].ToString()) + " " + (Triangles[1] == null ? "x" : Triangles[1].ToString())
                + "("+ Points[0] + " : " + Points[1] + ")";
        }
    }
}
