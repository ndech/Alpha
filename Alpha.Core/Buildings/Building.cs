using Alpha.Core.Events;
using Alpha.Core.Provinces;
using Alpha.Core.Realms;
using Alpha.Core.Tags;

namespace Alpha.Core.Buildings
{
    public class Building : ITagable, IEventable
    {
        public BuildingType Type { get; internal set; }
        public Settlement Location { get; internal set; }
        public float Condition { get; internal set; }
        public TagCollection Tags { get; private set; }

        public BuildingStatus Status
        {
            get { return BuildingStatuses.For(this); }
        }

        internal Building(Settlement location, BuildingType type)
        {
            Location = location;
            Type = type;
            Condition = 1;
            Tags = new TagCollection();
        }

        public Realm ResponsibleRealm
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
