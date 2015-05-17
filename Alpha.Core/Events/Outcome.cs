using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Commands;
using Alpha.Core.Dynamic;
using Roslyn.Scripting;

namespace Alpha.Core.Events
{
    class Outcome<T>
    {
        private readonly Func<T, string> _labelFunc;
        private Func<T, string> _tooltîpFunc;
        private readonly IEnumerable<Func<T, Command>> _effects;
        private DynamicValue<T> _iaAffinity;
        private readonly IEnumerable<Condition<T>> _conditions;

        internal Outcome(XElement item)
        {
            _labelFunc = item.OptionalElement("label", e => Engine.GetFunc<T, string>(e.Value, Engine.NewSession), x=>"No label");
            Session session = Engine.NewSession;
            Engine.Execute<string>("using Alpha.Core.Commands;", session);
            _effects = item.Element("effects").Elements("effect").Select(e => Engine.GetFunc<T, Command>("new " + e.Value, session)).ToList();
        }

        public string Label(T item)
        {
            return _labelFunc(item);
        }

        public string Tooltip(T item)
        {
            return _tooltîpFunc == null ? null : _tooltîpFunc(item);
        }

        internal bool IsValid(T item)
        {
            return _conditions.All(c => c.IsValid(item));
        }

        public double IaAffinity(T item)
        {
            return _iaAffinity.For(item);
        }

        public IEnumerable<Command> Execute(T item)
        {
            return _effects.Select(e => e(item));
        }
    }
}
