using System;

namespace Alpha.Core.Events
{
    class DelayedEvent
    {
        public DelayedEvent(IEventable target, Event<IEventable> @event, int delay, object[] parameters)
        {
            Target = target;
            Event = @event;
            _delay = delay;
            Parameters = parameters;
        }
        
        private Int32 _delay;
        public IEventable Target { get; private set; }
        public Event<IEventable> Event { get; private set; }
        public Object[] Parameters { get; private set; }
        public bool IsReady { get { return _delay <= 0; } }

        public void DayUpdate()
        {
            _delay --;
        }
    }
}
