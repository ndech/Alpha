using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Realms;
using Alpha.Toolkit;

namespace Alpha.Core.Provinces
{
    public class LandProvince : Province
    {

        public LandProvince(World world, List<Zone> zones) : base(world, zones)
        {
            Name = NameGenerator.GetRandomProvinceName();
            Color = CustomColor.Random;
            Capital = new Settlement(world, zones.RandomItem(), this);
            foreach (Zone zone in zones.Except(Capital.Zone))
                _settlements.Add(new Settlement(world, zone, this));
        }
        
        protected override void DayUpdate()
        {
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
        public bool IsCoastal { get { return Zones.SelectMany(z => z.Neighbourgs).Any(z => z.Province is SeaProvince); } }
    }
}