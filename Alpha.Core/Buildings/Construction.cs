using System.Collections.Generic;
using Alpha.Core.Calendars;
using Alpha.Core.Provinces;
using Alpha.Toolkit;

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
            _resourceRequirements = _type.RequirementsFor(location);
        }

        void IDailyUpdatableItem.DayUpdate()
        {
            if (_constructionStage == ConstructionStage.ResourceGathering)
            {
                
            }
            else
            {
                
            }
        }
    }
}
