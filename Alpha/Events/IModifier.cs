namespace Alpha.Events
{
    interface IModifier<T>
    {
        double Modifier(T item);
    }
    enum ModifierType
    {
        Multiplier,
        Reducer
    }
}
