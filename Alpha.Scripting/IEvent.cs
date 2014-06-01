namespace Alpha.Scripting
{
    public interface IEvent
    {
        bool IsTriggeredOnly { get; }
        void Execute(IEventable item, object[] parameters = null);
    }

    public interface IEvent<T> : IEvent
    {
        
    }
}
