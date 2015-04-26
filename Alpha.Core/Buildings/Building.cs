using Alpha.Core.Provinces;

namespace Alpha.Core.Buildings
{
    public class Building
    {
        public BuildingType Type { get; internal set; }
        public Settlement Location { get; internal set; }
        internal float ConditionPercentage { get; set; }

        public BuildingStatus BuildingCondition
        {
            get { return BuildingStatus.For(this); }
        }
    }
}
