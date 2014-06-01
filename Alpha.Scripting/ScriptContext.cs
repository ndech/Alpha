using System;

namespace Alpha.Scripting
{
    public class ScriptContext
    {
        public IScriptableCalendar Calendar;
        private readonly IEventPropagator _eventManager;

        public ScriptContext(IScriptableCalendar calendar, IEventPropagator eventManager)
        {
            Calendar = calendar;
            _eventManager = eventManager;
        }

        public void TriggerEvent(IEventable target, String eventId, String delay, params object[] parameters)
        {
            _eventManager.Trigger(target, eventId, delay, parameters);
        }
    }
}
