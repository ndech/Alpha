using System.Collections.Generic;
using Alpha.Toolkit;

namespace Alpha.Core.Fleets
{
    public class FleetManager : Component, IManager
    {
        private readonly List<Fleet> _fleets = new List<Fleet>();
        public IEnumerable<Fleet> Fleets { get { return _fleets; } }

        internal FleetManager(World world) : base(world)
        { }

        void IDailyUpdatable.DayUpdate(DataLock dataLock)
        {
            _fleets.ForEach(fleet => dataLock.Write(() => (fleet as IDailyUpdatableItem).DayUpdate()));
        }

        public void CreateFleet(Fleet fleet)
        {
            _fleets.Add(fleet);
        }
    }
}