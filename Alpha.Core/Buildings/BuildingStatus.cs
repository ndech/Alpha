using System.Xml.Linq;
using Alpha.Core.Dynamic;
using Alpha.Core.Events;
using Alpha.Core.Provinces;

namespace Alpha.Core.Buildings
{
    class BuildingStatus
    {
        internal int Order { get; set; }
        public string Label { get; internal set; }
        
        public override string ToString()
        {
            return Label;
        }

        public Condition<Settlement> Condition { get; internal set; }

        public BuildingStatus(XElement element)
        {
            Order = element.MandatoryAttribute("order", "A building status must have an order.").ToInt();
            Label = element.MandatoryElement("label", "A building status must have a label.").Value;
            Condition = element.OptionalElement("condition", (e) => new Condition<Settlement>(e), new Condition<Settlement>(true));
        }
    }
}
