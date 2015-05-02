using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Alpha.Core.Buildings;
using Alpha.Core.Save;
using Alpha.Toolkit;

namespace Alpha.Core.Provinces
{
    public class Settlement : Component, IDailyUpdatableItem, ISavable
    {
        public String Name { get; private set; }
        public LandProvince Province { get; private set; }
        public BaseSettlementType Type { get; private set; }
        public List<Building> Buildings { get; internal set; }
        public List<Construction> Constructions { get; internal set; }
        public Population Population { get; internal set; }

        public Settlement(World world, LandProvince province) : base(world)
        {
            Province = province;
            Name = NameGenerator.GetSettlementName();
            Constructions = new List<Construction>();
            Buildings = new List<Building>();
            Population = new Population();
            Type = world.ProvinceManager.BaseSettlementTypes.RandomWeightedItem(t => t.Probability(province));
        }

        void IDailyUpdatableItem.DayUpdate()
        {
            Population.DayUpdate();
            Constructions.DayUpdate();
        }

        internal void ConstructionCompleted(Construction construction)
        {
            //Buildings.Add(construction.);
            Constructions.Remove(construction);
        }

        internal void StartConstruction(BuildingType building)
        {
            Constructions.Add(new Construction(building, this));
        }

        public override string ToString()
        {
            return Type.Name + " of " + Name;
        }
        
        public XElement Save()
        {
            return new XElement("settlement",
                        new XElement("name"));
        }

        public void Load()
        {
            throw new NotImplementedException();
        }
    }
}
