using System;
using Alpha.Scripting;

namespace Alpha.Events
{
    class DelayedEvent
    {
        public DelayedEvent(IEventable target, IEvent @event, int delay, object[] parameters)
        {
            Target = target;
            Event = @event;
            Delay = delay;
            Parameters = parameters;
        }

        public IEventable Target { get; protected set; }
        public IEvent Event { get; protected set; }
        public Int32 Delay { get; set; }
        public Object[] Parameters { get; protected set; }
    }
}
