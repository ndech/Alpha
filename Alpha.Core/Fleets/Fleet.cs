using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Scripting;

namespace Alpha.Core.Fleets
{
    partial class Fleet : IFleet, IScriptableFleet, IDailyUpdatable
    {
        public String Name { get; internal set; }
        public IList<Ship> Ships { get; private set; }
        public float Speed { get { return Ships.Min(s => s.Speed); } }
        public void DayUpdate(object dataLock)
        {
            lock (dataLock)
            {
                
            }
        }
    }
}
