using System;
using System.Collections.Generic;

namespace Alpha.Core.Realms
{
    class Realm : IDailyUpdatable
    {
        public Realm Liege { get; internal set; }

        private readonly List<Realm> _vassals = new List<Realm>();
        public IEnumerable<Realm> Vassals { get { return _vassals; } } 

        public String Name { get; internal set; }

        void IDailyUpdatable.DayUpdate(object dataLock)
        {

        }

        public bool IsIndependant { get { return Liege == null; } }
    }
}
