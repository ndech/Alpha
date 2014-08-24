using System.Collections.Generic;
using Alpha.Core;
using Alpha.Core.Commands;
using Alpha.Core.Notifications;
using Alpha.Core.Realms;
using Alpha.Toolkit;

namespace Alpha.AI
{
    public interface IAi
    {
        IEnumerable<Command> Process(DataLock dataLock, IEnumerable<Notification> notifications);
        Realm Realm { get; }
        RealmToken RealmToken { get; }
    }
}