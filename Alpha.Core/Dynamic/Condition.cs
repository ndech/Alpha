using System;
using System.Xml.Linq;

namespace Alpha.Core.Dynamic
{
    internal class Condition<T>
    {
        private readonly Func<T, bool> _expression;

        internal Condition(XElement element)
        {
            String scriptIdentifier = typeof(T).Name;
            Engine.Execute<Func<T, Double>>("(" + scriptIdentifier + ") => " + element.Value, Engine.NewSession);
        }

        public Condition(bool expression)
        {
            _expression = s => expression;
        }

        public bool IsValid(T item)
        {
            return _expression(item);
        }
    }
}