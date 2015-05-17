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
        private readonly DynamicValue<Settlement> _manpowerRequirement;
        
        internal List<ResourceRequirement> ResourceRequirementsFor(Settlement location)
        {
            return _resourceRequirements.Select(kvp => new ResourceRequirement(kvp.Key, (int)kvp.Value.For(location))).ToList();
        }

        internal int ManPowerRequirementsFor(Settlement location) => (int)_manpowerRequirement.For(location);

        internal ConstructionStep(BuildingType buildingType, XElement element)
        {
            _resourceRequirements = element.Descendants("resource")
                .Select(e => new
                {
                    Resource = ResourceTypes.Types.Single(t => t.Id.Equals(e.MandatoryAttribute("type", "A construction step has a resource with no type ("+buildingType.Id+").").Value)),
                    Value = new DynamicValue<Settlement>(e)
                })
                .ToDictionary(e => e.Resource, e => e.Value);
            _manpowerRequirement = new DynamicValue<Settlement>(element.MandatoryElement("manpower", "A construction step has no manpower requirement (" + buildingType.Id + ")."));
        }
    }
}
