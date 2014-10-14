using System;
using System.Xml.Linq;
using Alpha.Core.Dynamic;

namespace Alpha.Core.Provinces
{
    public class Building
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public DynamicValue<Settlement> ConstructionTime { get; set; }
        public DynamicValue<Settlement> Cost { get; set; } 

        
        internal Building(XElement element)
        {
            Id = element.Attribute("id").Value;
            Name = element.Element("name").Value;
            ConstructionTime = new DynamicValue<Settlement>(element.Element("constructionTime"));
            Cost = new DynamicValue<Settlement>(element.Element("cost"));
        }
    }
}
