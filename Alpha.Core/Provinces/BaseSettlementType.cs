using System;
using System.Xml.Linq;
using Alpha.Core.Dynamic;

namespace Alpha.Core.Provinces
{
    public class BaseSettlementType : SettlementType
    {
        private readonly DynamicValue<LandProvince> _probability;

        private BaseSettlementType(string id, string name, string description, DynamicValue<LandProvince> probability)
            : base(id, name, description)
        {
            _probability = probability;
        }

        internal static BaseSettlementType Create(XElement element)
        {
            string id = element.Attribute("id").Value;
            string name = element.Element("name").Value;
            string description = element.Element("description").Value;
            DynamicValue<LandProvince> probability = new DynamicValue<LandProvince>(element.Element("probability"));
            return new BaseSettlementType(id, name, description, probability);
        }

        public double Probability(LandProvince province)
        {
            return _probability.For(province);
        }
    }
}
