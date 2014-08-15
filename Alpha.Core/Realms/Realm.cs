using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Provinces;
using Alpha.Toolkit;

namespace Alpha.Core.Realms
{
    public class Realm : IDailyUpdatable
    {
        internal Realm()
        {
            Economy = new RealmEconomy();
        }

        public RealmEconomy Economy { get; private set; }
        public Realm Liege { get; internal set; }
        private readonly List<Realm> _vassals = new List<Realm>();
        private readonly List<LandProvince> _demesne = new List<LandProvince>();
        public IEnumerable<Realm> Vassals { get { return _vassals; } }
        public IEnumerable<LandProvince> Demesne { get { return _demesne; } } 
        //public IEnumerable<Fleet> Fleets { get { return World.FleetManager.Fleets.Where(f => f.Owner.Equals(this)); } } 
        
        public String Name { get; internal set; }

        void IDailyUpdatable.DayUpdate(object dataLock)
        {

        }

        public bool IsIndependant { get { return Liege == null; } }

        public Realm RandomDirectVassal { get { return _vassals.RandomItem(); } }

        public Realm RandomDirectVassalWhere(Func<Realm, bool> criteria)
        {
            return _vassals.Where(criteria).RandomItem();
        }
        public Int32 DemesneSize { get { return _demesne.Count; } }
        public Int32 DirectVassalCount { get { return _vassals.Count; } }
        public Int32 TotalVassalCount { get { return DirectVassalCount + Vassals.Sum((v) => v.TotalVassalCount); } }

        public void AddProvince(LandProvince province)
        {
            _demesne.Add(province);
            province.Owner = this;
        }
    }
}
