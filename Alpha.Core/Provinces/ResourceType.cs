using System;
using System.Xml.Linq;
using Alpha.Core.Dynamic;

namespace Alpha.Core.Provinces
{
    public class ResourceType
    {
        public String Id { get; private set; }
        public String Name { get; private set; }
        internal DynamicValue<Settlement> Probability { get; private set; }

        internal ResourceType(XElement element)
        {
            Id = element.Attribute("id").Value;
            Name = element.Element("name").Value;
            Probability = new DynamicValue<Settlement>(element.Element("probability"));
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
