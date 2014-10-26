using System;
using System.Xml.Linq;
using Alpha.Core.Dynamic;

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
        internal DynamicValue<Settlement> Probability { get; private set; }
        public ResourceCategory Category { get; private set; }


        internal ResourceType(XElement element)
        {
            Id = element.Attribute("id").Value;
            Name = element.Element("name").Value;
            Probability = new DynamicValue<Settlement>(element.Element("probability"));
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
