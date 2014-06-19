//using System;
//using Alpha.Toolkit.Math;

//namespace Alpha.WorldGeneration
//{
//    class Edge
//    {
//        public Vector2I a;
//        public Vector2I b;

//        public Triangle t1;
//        public Triangle t2;

//        public Edge(Vector2I a, Vector2I b)
//        {
//            this.a = a;
//            this.b = b;
//            t1 = Triangle.Exterior;
//            t2 = Triangle.Exterior;
//        }

//        public void Register(Triangle t)
//        {
//            if (t1 == Triangle.Exterior)
//                t1 = t;
//            else if(t2 == Triangle.Exterior)
//                t2 = t;
//            else
//                throw new Exception("All triangles already set");
//        }

//        public Triangle Other(Triangle t)
//        {
//            if(t1 == null || t2 == null)
//                throw new Exception("A triangle is null");
//            if (t == t1)
//                return t2;
//            else if (t == t2)
//                return t1;
//            else
//                throw new Exception("Invalid arguement");
//        }

//        public void Unregister(Triangle triangle)
//        {
//            if (t1 == triangle)
//                t1 = Triangle.Exterior;
//            else if (t2 == triangle)
//                t2 = Triangle.Exterior;
//            else
//                throw new Exception("No matching triangle to unregister");
//        }
//    }
//}
