using System;

namespace Alpha.Core.Dynamic
{
    internal class Condition<T>
    {
        private readonly Func<T, bool> _expression;

        internal Condition(Func<T, bool> expression)
        {
            _expression = expression;
        }

        public bool IsValid(T item)
        {
            return _expression(item);
        }
    }
}