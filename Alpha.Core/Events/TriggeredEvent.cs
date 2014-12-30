using System;
using System.Collections.Generic;
using Alpha.Core.Dynamic;

namespace Alpha.Core.Events
{
    class TriggeredEvent<T> : BaseEvent<T> where T : IEventable
    {
        public TriggeredEvent(string id, IEnumerable<Condition<T>> conditions) : base(id, conditions)
        { }

        public override bool IsTriggeredOnly { get { return true; } }

        public override void TryTrigger(T eventable)
        {
            throw new InvalidOperationException("Triggered only event can not be randomly triggered.");
        }
    }
}
