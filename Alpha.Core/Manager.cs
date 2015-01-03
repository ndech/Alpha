using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Events;
using Alpha.Toolkit;

namespace Alpha.Core
{
    public abstract class Manager : Component
    {
        internal Manager(World world) : base(world)
        {}
        internal abstract void Initialize();

        protected void TryTriggerEvents<T>(IEnumerable<IEvent<T>> events, IEnumerable<T> eventables, DataLock dataLock) where T : IEventable
        {
            foreach (T eventable in eventables)
                foreach (IEvent<T> e in events.Where(e=>!e.IsTriggeredOnly))
                    dataLock.Write(() =>
                    {
                        var result = e.TryTrigger(eventable);
                        if (result != null)
                            World.RegisterCommands(new RealmToken(World.RealmManager.Realms.RandomItem()), result);
                    });
        }

        internal abstract void DayUpdate(DataLock datalock);
    }
}