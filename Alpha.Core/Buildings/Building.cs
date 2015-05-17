using Alpha.Core.Events;
using Alpha.Core.Provinces;
using Alpha.Core.Realms;
using Alpha.Core.Tags;

namespace Alpha.Core.Buildings
{
    public class Building : ITagable, IEventable
    {
        public BuildingType Type { get; }
        public Settlement Location { get; }
        public float Condition { get; internal set; } = 1;
        public TagCollection Tags { get; } = new TagCollection();
        public BuildingStatus Status => BuildingStatuses.For(this);

        internal Building(Settlement location, BuildingType type)
        {
            Location = location;
            Type = type;
        }

        public Realm ResponsibleRealm
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
