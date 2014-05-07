namespace Alpha
{
    abstract class Event<T>
    {
        public abstract void Process(T item);
    }
}
