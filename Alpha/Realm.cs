using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Events;
using Alpha.Scripting;
using Alpha.Toolkit;

namespace Alpha
{
    class Realm : IDailyUpdatable, IScriptableRealm, IEventResolver
    {
        public double Treasury { get; set; }
        public double Income(string timeSpan)
        {
            return Revenue*TimeSpanParser.Parse(timeSpan);
        }

        public IScriptableRealm RandomDirectVassal { get { return Vassals.RandomItem(); } }
        public IScriptableRealm RandomDirectVassalWhere(Func<IScriptableRealm, bool> criteria)
        {
            return Vassals.Where(criteria).RandomItem();
        }

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
        
        public bool IsPlayer { get; set; }

        public int DirectVassalCount { get { return Vassals.Count; } }
        public int TotalVassalCount { get { return DirectVassalCount + Vassals.Sum((v) => v.TotalVassalCount); } }
        public bool IsIndependant { get { return Liege == null; } }
        public static String ScriptIdentifier { get { return "Realm"; } }
        public IEventResolver EventResolver { get { return this; } }
        public void Resolve<T>(T item, IEvent<T> @event) where T : IEventable
        {
            Event<T> eventRef = (Event<T>) @event;
            if (IsPlayer)
            {
                foreach (Outcome<T> outcome in eventRef.Outcomes)
                {
                    if (outcome.PreExecute != null) outcome.PreExecute.Invoke(item);
                    outcome.Effects.ToList().ForEach((e) => e.Invoke(item));
                }
            }
            else
            {
                double total = eventRef.Outcomes.Where((o) => o.ConditionsValid(item)).Sum((o) => o.IaAffinity(item));
                double rand = RandomGenerator.GetDouble(0.0, total);
                foreach (Outcome<T> outcome in eventRef.Outcomes.Where((o) => o.ConditionsValid(item)))
                {
                    if(outcome.PreExecute != null) outcome.PreExecute.Invoke(item);
                    double affinity = outcome.IaAffinity(item);
                    if (rand < affinity)
                    {
                        outcome.Effects.ToList().ForEach((e)=>e.Invoke(item));
                        return;
                    }
                    rand -= affinity;
                }
            }
        }
        
        public override String ToString()
        {
            return "Realm : " + Name;
        }
    }
}