using System.Collections.Generic;
using System.Linq;
using Alpha.Toolkit;

namespace Alpha.Core.Realms
{
    public class RealmManager : Manager
    {
        private readonly List<Realm> _realms = new List<Realm>();
        public IEnumerable<Realm> Realms { get { return _realms; } }
        public IEnumerable<Realm> IndependantsRealms { get
        {
            return _realms.Where(realm => realm.IsIndependant);
        } } 

        internal override void DayUpdate(DataLock dataLock)
        {
            _realms.DayUpdate(dataLock);
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
