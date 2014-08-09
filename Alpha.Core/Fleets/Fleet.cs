using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Realms;

namespace Alpha.Core.Fleets
{
    public class Fleet : IDailyUpdatable
    {
        internal Fleet()
        {
            
        }
        public String Name { get; internal set; }

        private readonly List<Ship> _ships = new List<Ship>();
        public IEnumerable<Ship> Ships { get { return _ships; } } 

        public Realm Owner { get; internal set; }

        public float Speed { get { return _ships.Min(s => s.Speed); } }

        void IDailyUpdatable.DayUpdate(object dataLock)
        {
            lock (dataLock)
            {
                
            }
        }
    }
}
