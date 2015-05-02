using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Calendars;
using Alpha.Core.Provinces;

namespace Alpha.Core.Buildings
{
    public class Construction : IDailyUpdatableItem
    {
        private readonly BuildingType _type;
        private readonly Settlement _location;
        private readonly Date _startDate;
        private readonly List<ResourceRequirement> _resourceRequirements; 
        private ConstructionStage _constructionStage = ConstructionStage.ResourceGathering;

        public enum ConstructionStage
        {
            ResourceGathering,
            Construction
        }

        public Construction(BuildingType type, Settlement location)
        {
            _type = type;
            _location = location;
            //_resourceRequirements = _type.RequirementsFor(location);
        }

        void IDailyUpdatableItem.DayUpdate()
        {
            if (_constructionStage == ConstructionStage.ResourceGathering)
            {
                if(_resourceRequirements.All(rr=>rr.IsFullfilled))
                    _constructionStage = ConstructionStage.Construction;
                foreach (ResourceRequirement requirement in _resourceRequirements)
                {
                    Resource resource = _location.Province.Resources.SingleOrDefault(r => r.Type == requirement.ResourceType);
                    if(resource == null)
                        continue;
                    double change = Math.Min(resource.StorageLevel, requirement.RemainingValue);
                    requirement.CurrentValue += change;
                    resource.StorageLevel -= change;
                }
            }
            else
            {
                
            }
        }
    }
}
