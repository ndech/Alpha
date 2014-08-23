using System.Collections.Generic;
using Alpha.Core.Commands;
using Alpha.Core.Notifications;
using Alpha.Toolkit;

namespace Alpha.AI
{
    public interface IAi
    {
        IList<Command> Process(DataLock dataLock, IEnumerable<Notification> notifications);
    }
}