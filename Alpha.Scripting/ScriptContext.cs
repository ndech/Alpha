using System;
using System.Collections.Generic;

namespace Alpha.Scripting
{
    public class ScriptContext
    {
        public IScriptableRealm PlayerRealm { get; private set; }
        public IList<IScriptableRealm> Realms { get; private set; }
        public IScriptableCalendar Calendar { get; private set; }
        private readonly IEventPropagator _eventManager;

        public ScriptContext(IScriptableCalendar calendar, IEventPropagator eventManager, IScriptableRealm playerRealm, IList<IScriptableRealm> realms)
        {
            PlayerRealm = playerRealm;
            Realms = realms;
            Calendar = calendar;
            _eventManager = eventManager;
        }

        public void TriggerEvent(IEventable target, String eventId, String delay, params object[] parameters)
        {
            _eventManager.Trigger(target, eventId, delay, parameters);
        }
    }
}
