using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Buildings;
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
        internal DynamicValue<LandProvince> Probability { get; private set; }
        public ResourceCategory Category { get; private set; }
        
        internal ResourceType(XElement element)
        {
            Id = element.Attribute("id").Value;
            Name = element.Element("name").Value;
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

        private static List<ResourceType> _resourceTypes;
        
        internal static void Initialize()
        {
            _resourceTypes = XDocument.Load(@"Data\Resources\Resources.xml")
                .Descendants("resource")
                .Select(x => new ResourceType(x))
                .ToList();
        }

        public static IEnumerable<ResourceType> Types
        {
            get
            {
                Debug.Assert(_resourceTypes != null, "Resource types not initialized");
                return _resourceTypes;
            }
        }
    }
}
