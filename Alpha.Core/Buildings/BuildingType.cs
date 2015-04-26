using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Dynamic;
using Alpha.Core.Events;
using Alpha.Core.Provinces;

namespace Alpha.Core.Buildings
{
    public class BuildingType
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public List<ConstructionStep> ConstructionSteps { get; set; } 

        internal BuildingType(XElement element, IEnumerable<ResourceType> resourceTypes)
        {
            Id = element.MandatoryAttribute("id", "A building must have an Id.").Value;
            Name = element.MandatoryElement("name", "A building type must have a name ("+Id+").").Value;
            ConstructionSteps = element
                    .MandatoryElement("constructionSteps", "A building type must have have at least one construction step (" + Id + ").")
                    .AtLeastOne("constructionStep", "A building type must have have at least one construction step (" + Id + ").")
                    .Select(e => new ConstructionStep(this, e, resourceTypes))
                    .ToList();
        }
    }
}
