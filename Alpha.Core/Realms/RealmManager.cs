using System.Collections.Generic;

namespace Alpha.Core.Realms
{
    public class RealmManager : IManager
    {
        private readonly List<Realm> _realms = new List<Realm>();
        public IEnumerable<Realm> Realms { get { return _realms; } } 

        void IDailyUpdatable.DayUpdate(object dataLock)
        {
            _realms.ForEach(realm=>((IDailyUpdatable)realm).DayUpdate(dataLock));
        }

        void IManager.Setup()
        {
            for (int i = 0; i < 10; i++)
                _realms.Add(new Realm());
        }

        internal RealmManager()
        {
            
        }
    }
}
