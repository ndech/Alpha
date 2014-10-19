﻿using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using Alpha.WorldGeneration;

namespace Alpha.Core.Provinces
{
    public abstract class Province : Component, IDailyUpdatableItem, IEquatable<Province>
    {
        public Cluster Cluster { get; set; }

        protected Province(World world, List<Zone> zones, Cluster cluster) : base(world)
        {
            Cluster = cluster;
            _zones = zones;
            Center = zones.RandomItem().Center;
            NumericId = IdSequence;
            Id = GenerateStringId(NumericId);
        }

        public abstract string Name { get; internal set; }

        protected abstract string GenerateStringId(int id);

        private static int _idSequence;
        protected static int IdSequence { get { return ++_idSequence; } }
        public int NumericId { get; set; }
        public String Id { get; private set; }

        private readonly List<Zone> _zones = new List<Zone>();
        public IEnumerable<Zone> Zones { get { return _zones; } }

        private readonly List<ProvinceAdjacency> _adjacencies = new List<ProvinceAdjacency>();
        public IEnumerable<ProvinceAdjacency> Adjacencies { get { return _adjacencies; } }
        public Vector3D Center { get; private set; }

        public bool Equals(Province other)
        {
            return Id == other.Id;
        }

        protected abstract void DayUpdate();
        void IDailyUpdatableItem.DayUpdate()
        {
            DayUpdate();
        }

        public void CreateAdjacency(ProvinceAdjacency provinceAdjacency)
        {
            _adjacencies.Add(provinceAdjacency);
        }

        public override string ToString()
        {
            return Name;
        }

        public double DistanceWith(Province other)
        {
            return Vector3D.Distance(Center, other.Center);
        }

        public IEnumerable<Province> Neighbourgs { get { return Adjacencies.Select(a => a.Neighbourg); } } 
    }
}
