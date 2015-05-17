using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Provinces;
using Alpha.Toolkit;

namespace Alpha.Core.Buildings
{
    public static class BuildingTypes
    {
        private static List<BuildingType> _buildingTypes;

        internal static void Initialize()
        {
            XDocument buildingsDefinition = XDocument.Load(@"Data\Buildings\BuildingTypes.xml");
            _buildingTypes = buildingsDefinition.Descendants("building").Select(x => new BuildingType(x)).ToList();
            var links = buildingsDefinition.Descendants("replaces")
                .Elements("replace")
                .Select(e => new {From = GetById(e.Value), To = GetById(e.Parent.Parent.Attribute("id").Value)}).ToList();
            foreach (var group in links.GroupBy(x => x.From))
                group.Key.SetUnlocksBuildings(group.Select(g => g.To));
            foreach (var group in links.GroupBy(x => x.To))
                group.Key.SetReplaceBuildings(group.Select(g => g.From));

        }

        public static IEnumerable<BuildingType> AvailableFor(Settlement settlement)
        {
            Debug.Assert(_buildingTypes != null, "Building statuses not initialized");
            return
                _buildingTypes
                    .Except(settlement.Buildings.Select(b => b.Type)) //Not already built
                    .Where(type => (!type.ReplaceBuildings.Any() || type.ReplaceBuildings.Any(t=>settlement.Buildings.Select(b=>b.Type).Contains(t))))
                    .Where(type => type.PreConditionsValidFor(settlement));
        }

        public static BuildingType GetById(string id)
        {
            Debug.Assert(_buildingTypes != null, "Building statuses not initialized");
            return _buildingTypes.Single(b => b.Id.Equals(id), "No building type with id ("+id+") is defined.");
        }
    }
}