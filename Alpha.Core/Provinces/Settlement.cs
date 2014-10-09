using System;
using Alpha.Toolkit;

namespace Alpha.Core.Provinces
{
    public class Settlement
    {
        public String Name { get; private set; }
        public LandProvince Province { get; private set; }
        public int Population { get; private set; }
        public double Income { get; private set; }
        public String Type { get; private set; }

        public Settlement(LandProvince province)
        {
            Province = province;
            Name = NameGenerator.GetSettlementName();
            Population = RandomGenerator.Get(20, 5000);
            Income = RandomGenerator.GetDouble(-10, 10);
            Type = Population > 4000 ? "city" : "castle";
        }
    }
}
