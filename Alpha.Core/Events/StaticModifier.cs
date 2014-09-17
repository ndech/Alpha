using System;

namespace Alpha.Core.Events
{
    class StaticModifier<T> : IModifier<T>
    {
        private readonly double _factor;
        private readonly Func<T, bool> _expression;

        public StaticModifier(double factor, Func<T, bool> expression, ModifierType type = ModifierType.Multiplier)
        {
            _factor = (type == ModifierType.Multiplier ? factor : 1.0f/factor);
            _expression = expression;
        }

        double IModifier<T>.Modifier(T item)
        {
            return _expression(item) ? _factor : 1.0;
        }
    }
}