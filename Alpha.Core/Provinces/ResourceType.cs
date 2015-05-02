using System;
using System.Xml.Linq;
using Alpha.Core.Dynamic;
using Alpha.Core.Events;

namespace Alpha.Core.Provinces
{
    public class ResourceType
    {
        public enum ResourceCategory
        {
            Food,
            Building,
            Default
        }
        public String Id { get; private set; }
        public String Name { get; private set; }
        internal DynamicValue<LandProvince> Probability { get; private set; }
        public ResourceCategory Category { get; private set; }
        
        internal ResourceType(XElement element)
        {
            Id = element.MandatoryAttribute("id", "A resource type without id is defined.").Value;
            Name = element.MandatoryElement("name", "The resource type ("+Id+") has no name.").Value;
            Probability = new DynamicValue<LandProvince>(element.Element("probability"));
            ResourceCategory category = ResourceCategory.Default;
            bool conversionSucceeded = element.Element("category") != null;
            if(conversionSucceeded)
                conversionSucceeded = ResourceCategory.TryParse(element.Element("category").Value, true, out category);
            Category = conversionSucceeded ? category : ResourceCategory.Default;
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
