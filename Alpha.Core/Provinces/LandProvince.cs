using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Realms;
using Alpha.Toolkit;

namespace Alpha.Core.Provinces
{
    public class LandProvince : Province
    {
        private double _population = 1000;
        public int PopulationLastDayVariation { get; private set; }

        public LandProvince(World world, List<Zone> zones) : base(world, zones)
        {
            _population = RandomGenerator.Get(1000, 100000);
            YearlyGrowth = RandomGenerator.GetDouble(-0.3, 0.9);
            Name = NameGenerator.GetRandomProvinceName();
            Color = CustomColor.Random;
            Capital = new Settlement(world, zones.RandomItem(), this);
            foreach (Zone zone in zones.Except(Capital.Zone))
                _settlements.Add(new Settlement(world, zone, this));
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

            foreach (Settlement settlement in AllSettlements)
                settlement.DayUpdate();
        }

        public override sealed string Name { get; internal set; }
        protected override string GenerateStringId(int id)
        {
            return "land_province_" + id;
        }

        public Realm Owner { get; internal set; }
        public CustomColor Color { get; internal set; }

        public Settlement Capital { get; internal set; }
        private readonly List<Settlement> _settlements = new List<Settlement>();
        public IEnumerable<Settlement> Settlements { get { return _settlements; } }
        public IEnumerable<Settlement> AllSettlements { get { return _settlements.Union(Capital); } }


        public IEnumerable<SettlementType> AvailableSettlementTypes { get
        {
            return World.ProvinceManager.BaseSettlementTypes;
        } }

        public bool IsCoastal { get { return Zones.SelectMany(z => z.Neighbourgs).Any(z => z.Province is SeaProvince); } }
    }
}