using System;
using System.Collections.Generic;
using Alpha.Scripting.Providers;

namespace Alpha.Scripting
{
    public class ScriptContext
    {
        public IScriptableRealm PlayerRealm { get; private set; }
        public IList<IScriptableRealm> Realms { get; private set; }
        public IList<IScriptableFleet> Fleets { get { return _fleetProvider.ScripableFleets; }} 
        public IScriptableCalendar Calendar { get; private set; }
        private readonly IEventPropagator _eventManager;
        private readonly IFleetProvider _fleetProvider;
       
        public ScriptContext(IScriptableCalendar calendar, IEventPropagator eventManager, IScriptableRealm playerRealm, IList<IScriptableRealm> realms, IFleetProvider fleetProvider )
        {
            PlayerRealm = playerRealm;
            Realms = realms;
            Calendar = calendar;
            _eventManager = eventManager;
            _fleetProvider = fleetProvider;
        }

        public void TriggerEvent(IEventable target, String eventId, String delay, params object[] parameters)
        {
            _eventManager.Trigger(target, eventId, delay, parameters);
        }
    }
}
