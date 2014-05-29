﻿using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Scripting;

namespace Alpha
{
    class Realm : IDailyUpdatable, IScriptableRealm
    {
        public double Treasury { get; set; }
        public double Revenue { get { return TaxIncome + VassalsIncome - Spending; } }
        public double TaxIncome { get { return Demesne.Sum(d => d.BaseTax)*TaxRate; } }
        public double VassalsIncome { get { return Vassals.Sum(v => v.LiegeTax); } }
        public double Spending { get { return 1000*SpendingRate; } }
        public double LiegeTax { get { return (TaxIncome + VassalsIncome)*(Liege == null ? 0 : Liege.VassalTaxRate); } }
        public double TaxRate { get; set; }
        public double VassalTaxRate { get; set; }
        public double SpendingRate { get; set; }

        public IList<Realm> Vassals { get; private set; }

        public IList<Territory> Demesne { get; private set; }

        private Character _ruler;

        public Character Ruler
        {
            get { return _ruler; }
            set
            {
                if (_ruler != null)
                    _ruler.Realm = null;
                _ruler = value;
                if (_ruler != null)
                    _ruler.Realm = this;
            }
        }

        private Realm _liege;
        public Realm Liege
        {
            get
            {
                return _liege;
            }
            set
            {
                if (_liege != null)
                    _liege.Vassals.Remove(this);
                _liege = value;
                if(_liege != null)
                    _liege.Vassals.Add(this);
            }
        }

        public string Name { get; set; }

        public Realm(String name, Character ruler)
        {
            Name = name;
            Ruler = ruler;
            TaxRate = 0.0f;
            SpendingRate = 0.0f;
            Vassals = new List<Realm>();
            Demesne = new List<Territory>();
            Liege = null;
            Treasury = 50;
        }

        public void DayUpdate()
        {
            Treasury += Revenue;
        }

        public int DirectVassalCount { get { return Vassals.Count; } }
        public int TotalVassalCount { get { return DirectVassalCount + Vassals.Sum((v) => v.TotalVassalCount); } }
        public bool IsIndependant { get { return Liege == null; } }
    }
}