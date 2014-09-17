namespace Alpha.Scripting
{
    public interface IEventResolver<in T1, in T2> where T2 : IEvent<T1> where T1 : IEventable
    {
        void Resolve(T1 item, T2 @event);
    }
}