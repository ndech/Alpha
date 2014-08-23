using System.Collections.Generic;
using Alpha.Core.Notifications;
using Alpha.Toolkit;

namespace Alpha.Core.Fleets
{
    public class FleetManager : IManager
    {
        private readonly List<Fleet> _fleets = new List<Fleet>();
        public IEnumerable<Fleet> Fleets { get { return _fleets; } } 

        internal FleetManager()
        {

        }

        void IDailyUpdatable.DayUpdate(DataLock dataLock)
        {
            _fleets.ForEach(fleet => dataLock.Write(() => (fleet as IDailyUpdatableItem).DayUpdate()));
        }

        public void CreateFleet(Fleet fleet)
        {
            _fleets.Add(fleet);
            new NewFleetNotification(fleet);
        }
    }
}