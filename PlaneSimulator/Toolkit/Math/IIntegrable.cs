namespace PlaneSimulator.Toolkit.Math
{
    internal interface IIntegrable<T>
    {
        T Times(double multiplicator);
        T Add(T other);
    }
}