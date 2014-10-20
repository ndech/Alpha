using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Toolkit;

namespace Alpha.Core.Provinces
{
    public class Settlement : Component, IDailyUpdatableItem
    {
        public String Name { get; private set; }
        public Zone Zone { get; private set; }
        public LandProvince Province { get; private set; }
        public BaseSettlementType Type { get; private set; }
        public int Population { get; internal set; }
        public double Income { get; private set; }
        public List<Building> Buildings { get; internal set; }
        public List<Construction> Constructions { get; internal set; }

        public Settlement(World world, Zone zone, LandProvince province) : base(world)
        {
            Zone = zone;
            Province = province;
            Name = NameGenerator.GetSettlementName();
            Population = RandomGenerator.Get(1000, 6000);
            Income = RandomGenerator.GetDouble(-10, 10);
            Constructions = new List<Construction>();
            Buildings = new List<Building>();
            Type = world.ProvinceManager.BaseSettlementTypes.RandomWeightedItem(t => t.Probability(zone));
        }

        void IDailyUpdatableItem.DayUpdate()
        {
            Population ++;
            Constructions.DayUpdate();
        }

        internal void ConstructionCompleted(Construction construction)
        {
            Buildings.Add(construction.Building);
            Constructions.Remove(construction);
        }

        internal void StartConstruction(Building building)
        {
            Province.Owner.Pay(building.Cost(this));
            Constructions.Add(new Construction(building, this));
        }

        public override string ToString()
        {
            return Type.Name + " of " + Name;
        }

        private readonly List<Resource> _resources = new List<Resource>();
        public IEnumerable<Resource> Resources { get { return _resources; } }

        internal void AddResource(ResourceType type)
        {
            _resources.Add(new Resource(type));
        }

        public bool HasResource(String key)
        {
            return Resources.Any(r => r.Type.Id == key);
        }
    }
}
