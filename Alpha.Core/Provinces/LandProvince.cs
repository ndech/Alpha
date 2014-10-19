using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Notifications;
using Alpha.Core.Realms;
using Alpha.Toolkit;
using Alpha.WorldGeneration;

namespace Alpha.Core.Provinces
{
    public class LandProvince : Province
    {
        private double _population = 1000;
        public int PopulationLastDayVariation { get; private set; }

        public LandProvince(World world, List<Zone> zones, Cluster cluster) : base(world, zones, cluster)
        {
            _population = RandomGenerator.Get(1000, 100000);
            YearlyGrowth = RandomGenerator.GetDouble(-0.3, 0.9);
            Name = NameGenerator.GetRandomProvinceName();
            Color = new Tuple<float, float, float>(
                (float)RandomGenerator.GetDouble(0, 1), 
                (float)RandomGenerator.GetDouble(0, 1),
                (float)RandomGenerator.GetDouble(0, 1));
            Resources = new List<Resource>();
            //for (int i = 0; i < RandomGenerator.Get(1, 8); i++)
            //    FoundSettlement();
        }

        public Int32 Population
        {
            get { return (int)_population; }
            set { _population = Math.Max(0.0f, value + (_population - Population)); }
        }
        public double YearlyGrowth { get; private set; }
        public float BaseTax { get { return Population * 0.1f; } }

        protected override void DayUpdate()
        {
            double population = _population * (1+(RandomGenerator.GetDouble(YearlyGrowth-0.5, YearlyGrowth+0.5) / 365));
            PopulationLastDayVariation = (int)population - (int)_population;
            _population = population;

            foreach (Settlement settlement in Settlements)
                settlement.DayUpdate();
        }

        public override sealed string Name { get; internal set; }
        protected override string GenerateStringId(int id)
        {
            return "land_province_" + id;
        }

        public Realm Owner { get; internal set; }
        public Tuple<float, float, float> Color { get; internal set; }

        private readonly List<Settlement> _settlements = new List<Settlement>();
        public IEnumerable<Settlement> Settlements { get { return _settlements; } }

        public Settlement FoundSettlement(SettlementType type)
        {
            Owner.Economy.Treasury -= type.Cost;
            Settlement settlement = new Settlement(this, type);
            _settlements.Add(settlement);
            World.Notify(new NewSettlementNotification(settlement));
            return settlement;
        }

        public IEnumerable<SettlementType> AvailableSettlementTypes { get
        {
            return World.ProvinceManager.SettlementTypes;
        } }

        public bool IsCoastal { get { return Adjacencies.Any(a => a.Neighbourg is SeaProvince); } }
        public List<Resource> Resources { get; set; }

        public void AddResource(ResourceType type)
        {
            Resources.Add(new Resource(type));
        }

        public bool HasResource(String key)
        {
            return Resources.Any(r => r.Type.Id == key);
        }
    }
}