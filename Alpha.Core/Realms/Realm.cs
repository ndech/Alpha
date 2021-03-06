﻿using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Provinces;
using Alpha.Toolkit;

namespace Alpha.Core.Realms
{
    public class Realm : IDailyUpdatableItem
    {
        internal Realm(string name)
        {
            Economy = new RealmEconomy();
            Name = name;
            Id = IdSequence;
            Color = CustomColor.Random;
        }
        
        public RealmEconomy Economy { get; }
        public Realm Liege { get; internal set; }
        private readonly List<Realm> _vassals = new List<Realm>();
        private readonly List<LandProvince> _demesne = new List<LandProvince>();
        public IEnumerable<Realm> Vassals { get { return _vassals; } }
        public IEnumerable<LandProvince> Demesne { get { return _demesne; } }
        public IEnumerable<LandProvince> AllDependantProvinces { get
        {
            return _demesne.Union(_vassals.SelectMany(v => v.AllDependantProvinces));
        } } 
        //public IEnumerable<Fleet> Fleets { get { return World.FleetManager.Fleets.Where(f => f.Owner.Equals(this)); } } 
        
        public string Name { get; }

        void IDailyUpdatableItem.DayUpdate()
        {
            (Economy as IDailyUpdatableItem).DayUpdate();
        }

        public bool IsIndependant
        {
            get
            {
                return Liege == null;
            }
        }

        public Realm RandomDirectVassal { get { return _vassals.RandomItem(); } }

        public Realm RandomDirectVassalWhere(Func<Realm, bool> criteria)
        {
            return _vassals.Where(criteria).RandomItem();
        }
        public int DemesneSize { get { return _demesne.Count; } }
        public int DirectVassalCount { get { return _vassals.Count; } }
        public int TotalVassalCount { get { return DirectVassalCount + Vassals.Sum((v) => v.TotalVassalCount); } }

        public void AddProvince(LandProvince province)
        {
            _demesne.Add(province);
            province.Owner = this;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj) && ReferenceEquals(null, this)) return true;
            if (ReferenceEquals(null, obj) || ReferenceEquals(null, this)) return false;
            if (obj is RealmToken)
                return Id == ((RealmToken) obj).Realm.Id;
            if (obj is Realm)
                return Id == ((Realm) obj).Id;
            return false;
        }

        public override int GetHashCode()
        {
            return (Name != null ? Id.GetHashCode() : 0);
        }

        public static bool operator ==(Realm one, Realm other)
        {
            return !ReferenceEquals(null, one) && one.Equals(other);
        }

        public static bool operator !=(Realm one, Realm other)
        {
            return !(one == other);
        }
        public static bool operator ==(Realm one, RealmToken other)
        {
            if (ReferenceEquals(null, one) && ReferenceEquals(null, other)) return true;
            return !ReferenceEquals(one, null) && !ReferenceEquals(other, null) && one.Equals(other.Realm);
        }

        public static bool operator !=(Realm one, RealmToken other)
        {
            return !(one == other);
        }
        private static int _idSequence;
        protected static int IdSequence { get { return ++_idSequence; } }
        public int Id { get; }
        public CustomColor Color { get; internal set; }
        public override string ToString()
        {
            return Name;
        }

        public void Pay(int cost)
        {
            Economy.Treasury -= cost;
        }

        public bool CanAfford(int cost)
        {
            return Economy.Treasury >= cost;
        }
    }
}
