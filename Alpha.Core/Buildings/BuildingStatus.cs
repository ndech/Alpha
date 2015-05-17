using System.Xml.Linq;
using Alpha.Core.Dynamic;
using Alpha.Core.Events;

namespace Alpha.Core.Buildings
{
    public class BuildingStatus
    {
        internal int Order { get; }
        private string Label { get; }
        private Condition<Building> Condition { get; }
        

        internal BuildingStatus(XElement element)
        {
            Order = element.MandatoryAttribute("order", "A building status must have an order.").ToInt();
            Label = element.MandatoryElement("label", "A building status must have a label.").Value;
            Condition = element.OptionalElement("condition", (e) => new Condition<Building>(e), new Condition<Building>(true));
        }

        public override string ToString() => Label;
        internal bool IsValid(Building building) => Condition.IsValid(building);
    }
}
