using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Provinces;

namespace Alpha.Core.Buildings
{
    public static class BuildingTypes
    {
        private static List<BuildingType> _buildingTypes;

        internal static void Initialize()
        {
            _buildingTypes = XDocument.Load(@"Data\Buildings\BuildingTypes.xml").Descendants("building").Select(x => new BuildingType(x)).ToList();
        }

        public static IEnumerable<BuildingType> AvailableFor(Settlement settlement)
        {
            Debug.Assert(_buildingTypes != null, "Building statuses not initialized");
            return
                _buildingTypes.Except(settlement.Buildings.Select(b => b.Type))
                    .Where(type => type.PreConditionsValidFor(settlement));
        }
    }
}