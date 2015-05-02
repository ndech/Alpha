using Alpha.Core.Provinces;
using Alpha.Core.Tags;

namespace Alpha.Core.Buildings
{
    public class Building : ITagable
    {
        public BuildingType Type { get; internal set; }
        public Settlement Location { get; internal set; }
        public float Condition { get; set; }

        public BuildingStatus Status
        {
            get { return BuildingStatuses.For(this); }
        }

        public TagCollection Tags
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
