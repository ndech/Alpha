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
        private readonly Func<T, String> _labelFunc;
        private Func<T, String> _tooltîpFunc;
        private readonly IEnumerable<Func<T, Command>> _effects;
        private DynamicValue<T> _iaAffinity;
        private readonly IEnumerable<Condition<T>> _conditions;

        internal Outcome(XElement item)
        {
            String identifier = typeof (T).Name;
            _labelFunc = Engine.Execute<Func<T, String>>("(" + identifier + ")=>" + item.Element("label").Value, Engine.NewSession);
            Session session = Engine.NewSession;
            Engine.Execute<String>("using Alpha.Core.Commands;", session);
            _effects = item.Element("effects").Elements("effect").Select(e => Engine.Execute<Func<T, Command>>("(" + identifier + ")=> new " + e.Value, session)).ToList();
        }

        public String Label(T item)
        {
            return _labelFunc(item);
        }

        public String Tooltip(T item)
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
