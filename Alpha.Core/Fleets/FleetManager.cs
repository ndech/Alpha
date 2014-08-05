using System.Collections.Generic;

namespace Alpha.Core.Fleets
{
    class FleetManager : IFleetManager, IDailyUpdatable
    {
        IEnumerable<IFleet> IFleetManager.Fleets { get { return Fleets; } } 
        private IList<Fleet> Fleets { get; set; }

        public FleetManager()
        {
            Fleets = new List<Fleet>();
        }

        public void DayUpdate(object dataLock)
        {
            foreach (Fleet fleet in Fleets)
                fleet.DayUpdate(dataLock);
        }
    }
}