using System;
using System.Collections.Generic;
using Alpha.Core.Commands;
using Alpha.Core.Dynamic;

namespace Alpha.Core.Events
{
    class TriggeredEvent<T> : BaseEvent<T> where T : IEventable
    {
        public TriggeredEvent(string id, IEnumerable<Condition<T>> conditions, IEnumerable<Outcome<T>> outcomes) : base(id, conditions, outcomes)
        { }

        public override bool IsTriggeredOnly { get { return true; } }

        public override IEnumerable<Command> TryTrigger(T eventable)
        {
            throw new InvalidOperationException("Triggered only event can not be randomly triggered.");
        }
    }
}
