using System;
using System.Xml.Linq;

namespace Alpha.Core.Dynamic
{
    internal class Condition<T>
    {
        private readonly Func<T, bool> _expression;

        internal Condition(XElement element)
        {
            _expression = Engine.GetFunc<T, bool>(element.Value, Engine.NewSession);
        }

        public Condition(bool fixedValue)
        {
            _expression = t => fixedValue;
        }

        public bool IsValid(T item) => _expression(item);
    }
}