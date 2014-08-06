using System.Collections.Generic;

namespace Alpha.Core.Fleets
{
    public class FleetManager : IManager
    {
        public List<Fleet> Fleets { get; private set; }

        public FleetManager()
        {
            Fleets = new List<Fleet>();
        }

        void IDailyUpdatable.DayUpdate(object dataLock)
        {
            foreach (Fleet fleet in Fleets)
                (fleet as IDailyUpdatable).DayUpdate(dataLock);
        }
    }
}