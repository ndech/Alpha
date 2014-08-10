using System;
using System.Collections.Generic;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;

namespace Alpha.Core.Provinces
{
    public abstract class Province : IDailyUpdatable, IEquatable<Province>
    {
        protected Province(List<Zone> zones, String id)
        {
            _zones = zones;
            Center = zones.RandomItem().Center;
            Id = id;
        }
        public abstract string Name { get; internal set; }

        private static int _idSequence = 0;
        protected static int IdSequence { get { return ++_idSequence; } }

        private readonly List<Zone> _zones = new List<Zone>();
        public IEnumerable<Zone> Zones { get { return _zones; } }

        private readonly List<ProvinceAdjacency> _adjacencies = new List<ProvinceAdjacency>();
        public IEnumerable<ProvinceAdjacency> Adjacencies { get { return _adjacencies; } }
        public Vector3D Center { get; private set; }
        public String Id { get; private set; }

        void IDailyUpdatable.DayUpdate(object dataLock)
        {

        }

        public bool Equals(Province other)
        {
            return Id == other.Id;
        }
    }
}
