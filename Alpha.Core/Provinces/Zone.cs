using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Toolkit.Math;
using Alpha.WorldGeneration;

namespace Alpha.Core.Provinces
{
    public class Zone
    {
        private static int _idSequence;
        private static int IdSequence { get { return ++_idSequence; } }
        public Vector3D Center { get; private set; }
        public List<Vector3D> Points { get; private set; }
        public String Id { get; private set; }

        private readonly List<ZoneAdjacency> _adjacencies = new List<ZoneAdjacency>();
        public IEnumerable<ZoneAdjacency> Adjacencies { get { return _adjacencies; } }
        public IEnumerable<Zone> Neighbourgs { get { return Adjacencies.Select(a => a.Neighbourg); } }
        public Province Province { get; internal set; }
        
        public Zone(Vector3D center, IEnumerable<Vector3D> points)
        {
            Points = points.ToList();
            Center = center;
            Id = "zone_" + IdSequence;
        }

        public static explicit operator Zone(VoronoiSite site)
        {
            return new Zone(site.Center, site.Points);
        }

        public void AddAdjacency(ZoneAdjacency adjacency)
        {
            _adjacencies.Add(adjacency);
        }

        public double DistanceWith(Zone other)
        {
            return Vector3D.Distance(Center, other.Center);
        }
    }
}
