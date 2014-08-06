using System.Collections.Generic;

namespace Alpha.Core.Realms
{
    public class RealmManager : IManager
    {
        readonly List<Realm> _realms = new List<Realm>();
        IEnumerable<Realm> Realms { get { return _realms; } } 

        public void DayUpdate(object dataLock)
        {
            _realms.ForEach(realm=>((IDailyUpdatable)realm).DayUpdate(dataLock));
        }
    }
}
