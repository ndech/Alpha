using System.Collections.Generic;
using Alpha.Core.Events;
using Alpha.Toolkit;

namespace Alpha.Core.Fleets
{
    public class FleetManager : Manager
    {
        private readonly List<Fleet> _fleets = new List<Fleet>();
        private List<IEvent<Fleet>> _events; 
        public IEnumerable<Fleet> Fleets { get { return _fleets; } }

        internal FleetManager(World world) : base(world)
        { }

        internal override void DayUpdate(DataLock dataLock)
        {
            TryTriggerEvents(_events, _fleets, dataLock);
            _fleets.ForEach(fleet => dataLock.Write(() => (fleet as IDailyUpdatableItem).DayUpdate()));
        }

        internal override void Initialize()
        {
            _events = World.EventManager.LoadEvents<Fleet>();
        }

        public void CreateFleet(Fleet fleet)
        {
            _fleets.Add(fleet);
        }
    }
}