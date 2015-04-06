﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Dynamic;
using Alpha.Core.Events;
using Alpha.Core.Provinces;

namespace Alpha.Core.Buildings
{
    public class BuildingType
    {
        public String Id { get; set; }
        public String Name { get; set; }

        private readonly Dictionary<ResourceType, DynamicValue<Settlement>> _resourceRequirements; 

        internal BuildingType(XElement element, IEnumerable<ResourceType> resourceTypes)
        {
            Id = element.MandatoryAttribute("id", "A building must have an Id.").Value;
            Name = element.MandatoryElement("name", "A building type must have a name ("+Id+").").Value;
            _resourceRequirements = element.MandatoryElement("name", "A building type must have a resources tag (" + Id + ").").Elements("resource")
                .Select(e => new { 
                    Resource = resourceTypes.Single(t => t.Id.Equals(e.MandatoryAttribute("type", "A building type has an invalid resource (" + Id + ").").Value)), 
                    Value = new DynamicValue<Settlement>(e) })
                .ToDictionary(e => e.Resource, e => e.Value);
        }
        
        internal List<ResourceRequirement> RequirementsFor(Settlement location)
        {
            return _resourceRequirements.Select(kvp => new ResourceRequirement(kvp.Key, (int) kvp.Value.For(location))).ToList();
        }
    }
}