namespace Alpha.Core.Events
{
    public interface IEvent<in T>
        where T : IEventable
    {
        bool IsTriggeredOnly { get; }
        //void Execute(IEventable item, object[] parameters = null);
        void TryTrigger(T eventable);
    }
}
