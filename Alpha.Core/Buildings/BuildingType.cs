using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Dynamic;
using Alpha.Core.Events;
using Alpha.Core.Provinces;
using Alpha.Toolkit;

namespace Alpha.Core.Buildings
{
    public class BuildingType
    {
        public String Id { get; private set; }
        public String Name { get; private set; }
        public IReadOnlyCollection<ConstructionStep> ConstructionSteps { get; private set; }
        public IReadOnlyCollection<BuildingType> ReplaceBuildings { get; private set; }
        public IReadOnlyCollection<BuildingType> UnlocksBuildings { get; private set; }

        private readonly List<Condition<Settlement>> _preConditions;
        private List<Condition<Settlement>> _conditions; 

        internal BuildingType(XElement element)
        {
            Id = element.MandatoryAttribute("id", "A building must have an Id.").Value;
            Name = element.MandatoryElement("name", "A building type must have a name ("+Id+").").Value;
            ConstructionSteps = element
                    .MandatoryElement("constructionSteps", "A building type must have have at least one construction step (" + Id + ").")
                    .AtLeastOne("constructionStep", "A building type must have have at least one construction step (" + Id + ").")
                    .Select(e => new ConstructionStep(this, e))
                    .ToReadOnly();
            _preConditions = element.
                OptionalElement("preConditions", 
                    e => e.Elements("condition").Select(c=> new Condition<Settlement>(c)).ToList(),
                    new List<Condition<Settlement>>());
            _conditions = element.
                OptionalElement("conditions",
                    e => e.Elements("condition").Select(c => new Condition<Settlement>(c)).ToList(),
                    new List<Condition<Settlement>>());
        }

        public bool PreConditionsValidFor(Settlement settlement)
        {
            return _preConditions.All(p => p.IsValid(settlement));
        }

        public void SetReplaceBuildings(IEnumerable<BuildingType> types)
        {
            ReplaceBuildings = types.ToReadOnly();
        }

        public void SetUnlocksBuildings(IEnumerable<BuildingType> types)
        {
            UnlocksBuildings = types.ToReadOnly();
        }

        public override string ToString()
        {
            return Name + " ("+Id+")";
        }
    }
}
