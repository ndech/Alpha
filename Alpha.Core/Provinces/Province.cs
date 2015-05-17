using System;
using System.Collections.Generic;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;

namespace Alpha.Core.Provinces
{
    public abstract class Province : Component, IDailyUpdatableItem, IEquatable<Province>
    {

        protected Province(World world, List<Zone> zones) : base(world)
        {
            _zones = zones;
            Center = zones.RandomItem().Center;
            foreach (Zone zone in Zones)
                zone.Province = this;
            NumericId = IdSequence;
            Id = GenerateStringId(NumericId);
        }

        protected Province(World world, Zone zone)
            : this(world, new List<Zone>() { zone })
        {}

        public abstract string Name { get; internal set; }

        protected string GenerateStringId(int id)
        {
            return IdPrefix + "_"+ IdSequence;
        }
        protected abstract string IdPrefix { get; }

        private static int _idSequence;
        protected static int IdSequence { get { return ++_idSequence; } }
        public int NumericId { get; set; }
        public string Id { get; }

        private readonly List<Zone> _zones;
        public IEnumerable<Zone> Zones { get { return _zones; } }

        public Vector3D Center { get; private set; }

        public bool Equals(Province other)
        {
            return Id == other.Id;
        }

        protected virtual void DayUpdate() { }

        void IDailyUpdatableItem.DayUpdate()
        {
            DayUpdate();
        }
        
        public override string ToString()
        {
            return Name;
        }
    }
}
