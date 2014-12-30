using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Dynamic;

namespace Alpha.Core.Events
{
    public abstract class BaseEvent<T> : IEvent<T> where T : IEventable
    {
        public String Id { get; private set; }
        public abstract bool IsTriggeredOnly { get; }
        public abstract void TryTrigger(T eventable);
        private readonly IEnumerable<Condition<T>> _conditions;

        internal BaseEvent(String id, IEnumerable<Condition<T>> conditions)
        {
            Id = id;
            _conditions = conditions;
        }

        internal bool IsValid(T item)
        {
            return _conditions.All(c => c.IsValid(item));
        }

        public override string ToString()
        {
            return Id;
        }
    }
}