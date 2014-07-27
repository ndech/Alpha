using System;
using System.Collections.Generic;
using Alpha.Scripting;
using Alpha.Toolkit;
using SharpDX;

namespace Alpha
{
    public interface IProvince : IScriptableProvince
    { }

    abstract class Province : IProvince, IDailyUpdatable
    {
        private static int _idSequence = 0;
        protected static int IdSequence { get { return ++_idSequence; } }
        public List<Zone> Zones { get; set; }
        public Vector3 Center { get; private set; }
        public List<ProvinceAdjacency> Adjacencies { get; private set; }
        public String Id { get; private set; }
        public Province(List<Zone> zones, String id)
        {
            Zones = zones;
            Center = zones.RandomItem().Center;
            Id = id;
            Adjacencies = new List<ProvinceAdjacency>();
        }

        public abstract string Name { get; set; }

        public virtual void DayUpdate()
        { }
    }
}
