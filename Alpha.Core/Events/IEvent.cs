using System.Collections.Generic;
using Alpha.Core.Commands;

namespace Alpha.Core.Events
{
    public interface IEvent<in T>
        where T : IEventable
    {
        bool IsTriggeredOnly { get; }
        //void Execute(IEventable item, object[] parameters = null);
        IEnumerable<Command> TryTrigger(T eventable);
    }
}
