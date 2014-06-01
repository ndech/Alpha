namespace Alpha.Scripting
{
    public interface IEventPropagator
    {
        void Trigger(IEventable target, string eventId, string delay, object[] parameters);
    }
}
