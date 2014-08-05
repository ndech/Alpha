using System;
using System.Collections.Generic;
using System.Linq;

namespace Alpha.Core.Fleets
{
    public class Fleet : IDailyUpdatable
    {
        public String Name { get; internal set; }

        public List<Ship> Ships { get; private set; }

        public float Speed { get { return Ships.Min(s => s.Speed); } }

        void IDailyUpdatable.DayUpdate(object dataLock)
        {
            lock (dataLock)
            {
                
            }
        }
    }
}
