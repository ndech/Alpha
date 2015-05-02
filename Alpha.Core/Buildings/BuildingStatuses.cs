using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace Alpha.Core.Buildings
{
    class BuildingStatuses
    {
        private static List<BuildingStatus> _statuses;

        internal static void Initialize()
        {
            _statuses = XDocument.Load(@"Data\Buildings\BuildingStatuses.xml")
                .Descendants("buildingStatus")
                .Select(e => new BuildingStatus(e))
                .OrderBy(s => s.Order)
                .ToList();
        }

        internal static BuildingStatus For(Building building)
        {
            Debug.Assert(_statuses != null, "Building statuses not initialized");
            return _statuses.First(status => status.IsValid(building));
        }
    }
}
