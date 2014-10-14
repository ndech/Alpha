using System;
using System.Xml.Linq;
using Alpha.Core.Dynamic;

namespace Alpha.Core.Provinces
{
    public class Building
    {
        public String Id { get; set; }
        public String Name { get; set; }

        public int Cost(Settlement settlement)
        {
            return (int) _cost.For(settlement);
        }
        public int ConstructionDuration(Settlement settlement)
        {
            return (int)_constructionDuration.For(settlement);
        }

        private readonly DynamicValue<Settlement> _constructionDuration;
        private readonly DynamicValue<Settlement> _cost;
        
        internal Building(XElement element)
        {
            Id = element.Attribute("id").Value;
            Name = element.Element("name").Value;
            _constructionDuration = new DynamicValue<Settlement>(element.Element("constructionTime"));
            _cost = new DynamicValue<Settlement>(element.Element("cost"));
        }
    }
}
