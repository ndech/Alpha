using System;
using Alpha.Toolkit;

namespace Alpha.Core.Provinces
{
    public class Settlement : IDailyUpdatableItem
    {
        public String Name { get; private set; }
        public LandProvince Province { get; private set; }
        public int Population { get; internal set; }
        public double Income { get; private set; }
        public String Type { get; private set; }

        public Settlement(LandProvince province)
        {
            Province = province;
            Name = NameGenerator.GetSettlementName();
            Population = RandomGenerator.Get(1000, 6000);
            Income = RandomGenerator.GetDouble(-10, 10);
            Type = Population > 4000 ? "city" : "castle";
        }

        public void DayUpdate()
        {
            Population ++;
        }
    }
}
