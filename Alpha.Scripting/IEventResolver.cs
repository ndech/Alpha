namespace Alpha.Scripting
{
    public interface IEventResolver
    {
        void Resolve<T>(T item, IEvent<T> @event) where T : IEventable;
    }
}
