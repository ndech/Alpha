using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Commands;
using Alpha.Core.Dynamic;

namespace Alpha.Core.Events
{
    public abstract class BaseEvent<T> : IEvent<T> where T : IEventable
    {
        public String Id { get; private set; }
        public abstract bool IsTriggeredOnly { get; }
        public abstract IEnumerable<Command> TryTrigger(T eventable);
        internal readonly IEnumerable<Condition<T>> Conditions;
        internal readonly IEnumerable<Outcome<T>> Outcomes; 

        internal BaseEvent(String id, IEnumerable<Condition<T>> conditions, IEnumerable<Outcome<T>> outcomes)
        {
            Id = id;
            Conditions = conditions;
            Outcomes = outcomes;
        }

        internal bool IsValid(T item)
        {
            return Conditions.All(c => c.IsValid(item));
        }

        public override string ToString()
        {
            return Id;
        }
    }
}