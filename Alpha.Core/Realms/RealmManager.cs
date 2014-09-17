using System.Collections.Generic;
using Alpha.Toolkit;

namespace Alpha.Core.Realms
{
    public class RealmManager : Manager
    {
        private readonly List<Realm> _realms = new List<Realm>();
        public IEnumerable<Realm> Realms { get { return _realms; } } 

        internal override void DayUpdate(DataLock dataLock)
        {
            _realms.ForEach(realm=>dataLock.Write(()=>((IDailyUpdatableItem)realm).DayUpdate()));
        }

        internal RealmManager(World world) : base(world)
        { }

        public void CreateRealm(Realm realm)
        {
            _realms.Add(realm);
        }

        internal override void Initialize()
        { }
    }
}
