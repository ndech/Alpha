namespace PlaneSimulator.Toolkit.Math
{
    public interface IIntegrable<T>
    {
        T Times(double multiplicator);
        T Add(T other);
    }
}