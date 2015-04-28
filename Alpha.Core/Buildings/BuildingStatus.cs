using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Dynamic;
using Alpha.Core.Events;

namespace Alpha.Core.Buildings
{
    public class BuildingStatus
    {
        private int Order { get; set; }
        private string Label { get; set; }
        private Condition<Building> Condition { get; set; }
        
        public override string ToString()
        {
            return Label;
        }

        private BuildingStatus(XElement element)
        {
            Order = element.MandatoryAttribute("order", "A building status must have an order.").ToInt();
            Label = element.MandatoryElement("label", "A building status must have a label.").Value;
            Condition = element.OptionalElement("condition", (e) => new Condition<Building>(e), new Condition<Building>(true));
        }

        private static List<BuildingStatus> _statuses;

        internal static void Initialize()
        {
            _statuses = XDocument.Load(@"Data\Buildings\BuildingStatuses.xml")
                .Descendants("buildingStatus")
                .Select(e => new BuildingStatus(e))
                .OrderBy(s=>s.Order)
                .ToList();
        }

        internal static BuildingStatus For(Building building)
        {
            Debug.Assert(_statuses != null, "Building statuses not initialized");
            return _statuses.First(status => status.Condition.IsValid(building));
        }
    }
}
