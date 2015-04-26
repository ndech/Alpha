using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Dynamic;
using Alpha.Core.Events;
using Alpha.Core.Provinces;

namespace Alpha.Core.Buildings
{
    public class ConstructionStep
    {
        private readonly Dictionary<ResourceType, DynamicValue<Settlement>> _resourceRequirements;
        private DynamicValue<Settlement> _manpowerRequirement;
        

        internal List<ResourceRequirement> RequirementsFor(Settlement location)
        {
            return _resourceRequirements.Select(kvp => new ResourceRequirement(kvp.Key, (int)kvp.Value.For(location))).ToList();
        }

        internal ConstructionStep(XElement element, IEnumerable<ResourceType> resourceTypes)
        {
            _resourceRequirements = element.Descendants("resource")
                .Select(e => new
                {
                    Resource = resourceTypes.Single(t => t.Id.Equals(e.MandatoryAttribute("type", "A building type has an invalid resource.").Value)),
                    Value = new DynamicValue<Settlement>(e)
                })
                .ToDictionary(e => e.Resource, e => e.Value);
        }
    }
}
