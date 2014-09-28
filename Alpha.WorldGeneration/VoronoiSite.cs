using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using Alpha.Voronoi;

namespace Alpha.WorldGeneration
{
    public class VoronoiSite : IEquatable<VoronoiSite>
    {
        public VoronoiSite(Vector voronoiCenter)
        {
            VoronoiCenter = voronoiCenter;
            Edges = new List<VoronoiEdge>();
            Neighbourgs = new List<VoronoiSite>();
            VoronoiPoints = new List<Vector>();
            IsWater = false;
            IsOnBorder = false;
            ShoreDistance = DefaultShoreDistance;
            Id = idSeed++;
        }

        public Vector VoronoiCenter { get; private set; }
        public Vector3D Center { get { return new Vector3D(VoronoiCenter[0], 0.0f, VoronoiCenter[1]); } }
        
        public List<Vector> VoronoiPoints { get; private set; }

        public List<Vector3D> Points { get
        {
            return VoronoiPoints.Select(p => new Vector3D((float) p[0], 0.0f, (float) p[1])).ToList();
        } } 
        public List<VoronoiEdge> Edges { get; private set; }
        public bool IsOnBorder { get; private set; }
        public bool IsWater { get; set; }
        public List<VoronoiSite> Neighbourgs { get; set; }
        public int ShoreDistance { get; set; }
        public Cluster Cluster { get; set; }
        private static int idSeed=0;
        public int Id { get; set; }
        public string ProvinceId { get; set; }

        public const int DefaultShoreDistance = 10000;

        public void Reorder(int width, int height)
        {
            //Checks if the site is fully included in the frame :
            if (Edges.Select(e => e.VVertexA).All(p => IsVectorInRange(p, width, height)) &&
                Edges.Select(e => e.VVertexB).All(p => IsVectorInRange(p, width, height)))
                IsOnBorder = false;
            else
            {
                IsOnBorder = true;
                Vector previousIntersection = null;
                for (int i = Edges.Count - 1; i >= 0; i--)
                {
                    VoronoiEdge edge = Edges[i];
                    bool isAIn = IsVectorInRange(edge.VVertexA, width, height);
                    bool isBIn = IsVectorInRange(edge.VVertexB, width, height);
                    if (isAIn && isBIn) // Edge is fully in, nothing to do
                        continue;
                    if (isAIn || isBIn) // One point in in, needs to be clipped
                    {
                        Double slope = edge.DirectionVector[1]/edge.DirectionVector[0];
                        Vector pointIn = isAIn ? new Vector(edge.VVertexA) : new Vector(edge.VVertexB);
                        List<Vector> intersections = new List<Vector>();
                        //Test collision with (0,0) => (width,0) edge :
                        intersections.Add(new Vector(pointIn[0] - pointIn[1] / slope, 0));
                        //Test collision with (0,height) => (width,height) edge :
                        intersections.Add(new Vector(pointIn[0] + (height - pointIn[1]) / slope, height));
                        //Test collision with (0,0) => (0,height) edge :
                        intersections.Add(new Vector(0, pointIn[1] - pointIn[0] * slope));
                        //Test collision with (width,0) => (width,height) edge :
                        intersections.Add(new Vector(width, pointIn[1] + (width - pointIn[0]) * slope));
                        Vector intersection = intersections.MinBy(p => Vector.Dist(p, pointIn));
                        Edges.RemoveAt(i);
                        Edges.Add(new VoronoiEdge {VVertexA = intersection, VVertexB = pointIn});
                        if (previousIntersection == null)
                            previousIntersection = intersection;
                        else
                        {
                            if (intersection[0].Equals(previousIntersection[0]) ||
                                intersection[1].Equals(previousIntersection[1]))
                            {
                                Edges.Add(new VoronoiEdge { VVertexA = intersection, VVertexB = previousIntersection });
                                IsOnBorder = true;
                                previousIntersection = null;
                            }
                            else
                            {
                                List<Vector> corners = new List<Vector>
                                {
                                    new Vector(0, 0),
                                    new Vector(width, 0),
                                    new Vector(width, height),
                                    new Vector(0, height)
                                };
                                foreach (Vector corner in corners)
                                {
                                    if ((corner[0].Equals(previousIntersection[0]) ||
                                         corner[0].Equals(intersection[0])) &&
                                        (corner[1].Equals(previousIntersection[1]) ||
                                         corner[1].Equals(intersection[1])))
                                    {
                                        Edges.Add(new VoronoiEdge { VVertexA = corner, VVertexB = previousIntersection });
                                        Edges.Add(new VoronoiEdge { VVertexA = corner, VVertexB = intersection });
                                        IsOnBorder = true;
                                        break;
                                    }
                                }
                                previousIntersection = null;
                            }
                        }
                    }
                    else // Edge fully out, can be safely removed
                        Edges.RemoveAt(i);
                }
                if(previousIntersection!=null)
                    throw new InvalidOperationException("Error");
            }
            if (Edges.Count == 0)
                throw new InvalidOperationException("No edges");

            VoronoiEdge first = Edges[Edges.Count-1];
            Edges.RemoveAt(Edges.Count-1);
            Vector point;
            VoronoiPoints.Add(point = first.VVertexA);
            Edges.RemoveAll((e) => Equals(e.VVertexA, e.VVertexB));
            do
            {
                VoronoiEdge edge = Edges.Single(e => e.VVertexA.Equals(point) || e.VVertexB.Equals(point));
                Edges.Remove(edge);
                if(Equals(edge.VVertexA, point))
                    VoronoiPoints.Add(point = edge.VVertexB);
                else
                    VoronoiPoints.Add(point = edge.VVertexA);
            } while (!Equals(point, first.VVertexB));
            Edges.Clear();
            //Reverse ordering of the points if needed :
            Vector v1 =VoronoiPoints[0] - VoronoiPoints[1];
            Vector v2 = VoronoiCenter - VoronoiPoints[0];
            if (v1[0]*v2[1] - v1[1]*v2[0] > 0)
                VoronoiPoints.Reverse();
        }

        public bool IsVectorInRange(Vector vector, int width, int height)
        {
            return vector[0] > 0 && vector[0] < width && vector[1] > 0 && vector[1] < height;
        }

        public bool Equals(VoronoiSite other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
