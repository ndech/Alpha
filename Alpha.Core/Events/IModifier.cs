namespace Alpha.Core.Events
{
    interface IModifier<in T>
    {
        double Modifier(T item);
    }
    enum ModifierType
    {
        Multiplier,
        Reducer
    }
}
