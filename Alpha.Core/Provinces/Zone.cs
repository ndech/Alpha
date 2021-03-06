﻿using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using Alpha.WorldGeneration;

namespace Alpha.Core.Provinces
{
    public class Zone
    {
        private static int _idSequence;
        private static int IdSequence { get { return ++_idSequence; } }
        public Vector3D Center { get; }
        public bool IsWater { get; }
        public List<Vector3D> Points { get; }
        public string Id { get; private set; }

        private readonly List<ZoneAdjacency> _adjacencies = new List<ZoneAdjacency>();
        public IEnumerable<ZoneAdjacency> Adjacencies { get { return _adjacencies; } }
        public IEnumerable<Zone> Neighbourgs { get { return Adjacencies.Select(a => a.Neighbourg); } }
        public Province Province { get; internal set; }
        
        public Zone(Vector3D center, IEnumerable<Vector3D> points, bool isWater)
        {
            Points = points.ToList();
            Center = center;
            IsWater = isWater;
            Id = "zone_" + IdSequence;
        }

        public static explicit operator Zone(VoronoiSite site)
        {
            return new Zone(site.Center, site.Points, site.IsWater);
        }

        public void AddAdjacency(ZoneAdjacency adjacency)
        {
            _adjacencies.Add(adjacency);
        }

        public double DistanceWith(Zone other)
        {
            return Vector3D.Distance(Center, other.Center);
        }

        public bool IsCoastal { get { return Neighbourgs.Any(z => z.IsWater); } }

        public double Surface
        {
            get
            {
                return 0.5*Math.Abs(Points.Zip(Points.Jump(1), (a,b)=> new { From = a, To = b})
                    .Sum(segment => (segment.From.X * segment.To.Z) - (segment.From.Z * segment.To.X)));
            }
        }
    }
}
