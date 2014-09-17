using Alpha.Core.Realms;

namespace Alpha.Core.Events
{
    public interface IEventable
    {
        //IEventResolver<IEventable, IEvent<IEventable>> EventResolver { get; }
        Realm ResponsibleRealm { get; }
    }
}
