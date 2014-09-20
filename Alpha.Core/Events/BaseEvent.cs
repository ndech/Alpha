namespace Alpha.Core.Events
{
    abstract class BaseEvent<T> : IEvent<T> where T : IEventable
    {
        public abstract bool IsTriggeredOnly { get; }
        public abstract void TryTrigger(T eventable);
    }
}
