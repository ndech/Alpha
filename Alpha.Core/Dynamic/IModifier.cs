namespace Alpha.Core.Dynamic
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
