﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Alpha.Core.Buildings;
using Alpha.Core.Events;
using Alpha.Core.Realms;
using Alpha.Core.Save;
using Alpha.Toolkit;

namespace Alpha.Core.Provinces
{
    public class Settlement : Component, IDailyUpdatableItem, ISavable, IEventable
    {
        public string Name { get; }
        public LandProvince Province { get; private set; }
        public BaseSettlementType Type { get; }
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
            foreach (BuildingType type in BuildingTypes.AvailableFor(this))
                Buildings.Add(new Building(this, type));
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
            Constructions.Add(new Construction(building, this, World.Calendar.Today));
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

        public Realm ResponsibleRealm
        {
            get { throw new NotImplementedException(); }
        }
    }
}
