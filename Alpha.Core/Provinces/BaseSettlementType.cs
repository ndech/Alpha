using System;
using System.Xml.Linq;
using Alpha.Core.Dynamic;

namespace Alpha.Core.Provinces
{
    public class BaseSettlementType : SettlementType
    {
        private readonly DynamicValue<Zone> _probability;

        private BaseSettlementType(String id, String name, String description, DynamicValue<Zone> probability) : base(id, name, description)
        {
            _probability = probability;
        }

        internal static BaseSettlementType Create(XElement element)
        {
            string id = element.Attribute("id").Value;
            string name = element.Element("name").Value;
            string description = element.Element("description").Value;
            DynamicValue<Zone> probability = new DynamicValue<Zone>(element.Element("probability"));
            return new BaseSettlementType(id, name, description, probability);
        }

        public double Probability(Zone zone)
        {
            return _probability.For(zone);
        }
    }
}
