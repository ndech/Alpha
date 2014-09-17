using System;

namespace Alpha.Core.Events
{
    class DynamicModifier<T> : IModifier<T>
    {
        private readonly Func<T, double> _expression;
        private readonly ModifierType _type;

        public DynamicModifier(Func<T, Double> expression, ModifierType type = ModifierType.Multiplier)
        {
            _expression = expression;
            _type = type;
        }

        double IModifier<T>.Modifier(T item)
        {
            double value = _expression(item);
            return (_type == ModifierType.Reducer ? 1.0/value : value);
        }
    }
}