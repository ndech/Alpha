using System.Collections.Generic;

namespace Alpha.Core.Fleets
{
    public class FleetManager : IManager
    {
        private readonly List<Fleet> _fleets = new List<Fleet>();
        public IEnumerable<Fleet> Fleets { get { return _fleets; } } 

        internal FleetManager()
        {

        }

        void IDailyUpdatable.DayUpdate(object dataLock)
        {
            foreach (Fleet fleet in _fleets)
                (fleet as IDailyUpdatable).DayUpdate(dataLock);
        }

        void IManager.Setup()
        {

        }
    }
}