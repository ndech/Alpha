using System;
using System.Collections.Generic;
using Alpha.Toolkit;

namespace Alpha.Core.Provinces
{
    public class Settlement : IDailyUpdatableItem
    {
        public String Name { get; private set; }
        public LandProvince Province { get; private set; }
        public SettlementType Type { get; private set; }
        public int Population { get; internal set; }
        public double Income { get; private set; }
        public List<Building> Buildings { get; internal set; }
        public List<Construction> Constructions { get; internal set; } 

        public Settlement(LandProvince province, SettlementType type)
        {
            Province = province;
            Name = NameGenerator.GetSettlementName();
            Population = RandomGenerator.Get(1000, 6000);
            Income = RandomGenerator.GetDouble(-10, 10);
            Constructions = new List<Construction>();
            Type = type;
        }

        void IDailyUpdatableItem.DayUpdate()
        {
            Population ++;
            Constructions.DayUpdate();
        }

        public void ConstructionCompleted(Construction construction)
        {
            Buildings.Add(construction.Building);
            Constructions.Remove(construction);
        }
    }
}
