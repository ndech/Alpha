using System.Xml.Linq;
using Alpha.Core.Dynamic;
using Alpha.Core.Events;

namespace Alpha.Core.Buildings
{
    public class BuildingStatus
    {
        internal int Order { get; set; }
        private string Label { get; set; }
        private Condition<Building> Condition { get; set; }
        
        public override string ToString()
        {
            return Label;
        }

        internal BuildingStatus(XElement element)
        {
            Order = element.MandatoryAttribute("order", "A building status must have an order.").ToInt();
            Label = element.MandatoryElement("label", "A building status must have a label.").Value;
            Condition = element.OptionalElement("condition", (e) => new Condition<Building>(e), new Condition<Building>(true));
        }

        internal bool IsValid(Building building)
        {
            return Condition.IsValid(building);
        }
    }
}
