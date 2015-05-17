using System;
using System.Xml.Linq;

namespace Alpha.Core.Provinces
{
    public class ResourceLevel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }

        public ResourceLevel(XElement element)
        {
            Id = element.Attribute("id").Value;
            Name = element.Element("name").Value;
            Value = int.Parse(element.Element("value").Value);
        }
    }
}