using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Events;

namespace Alpha.Core.Dynamic
{
    internal class DynamicValue<T>
    {
        private readonly List<IModifier<T>> _modifiers;
        private readonly double _base;

        public double For(T item)
        {
            return _modifiers.Aggregate(_base, (value, multiplier) => value * multiplier.Modifier(item));
        }
        
        internal DynamicValue(XElement definition)
            : this(definition, s => double.Parse(s, CultureInfo.InvariantCulture))
        { }

        internal DynamicValue(XElement definition, Func<string, double> baseValueParser)
        {
            _base = baseValueParser(definition.MandatoryElement("base", "A dynamic value is defined without base.").Value);
            _modifiers = new List<IModifier<T>>();
            XElement xModifiers = definition.Element("modifiers");
            if (xModifiers == null) return;
            foreach (XElement xmlModifier in xModifiers.Elements())
            {
                ModifierType type;
                if (xmlModifier.Name == "multiplier")
                    type = ModifierType.Multiplier;
                else if (xmlModifier.Name == "reducer")
                    type = ModifierType.Reducer;
                else
                    continue;
                if (xmlModifier.Attribute("factor") == null)
                    _modifiers.Add(new DynamicModifier<T>(
                        Engine.GetFunc<T, double>(xmlModifier.Value, Engine.NewSession), type));
                else
                    _modifiers.Add(new StaticModifier<T>(double.Parse(xmlModifier.Attribute("factor").Value, CultureInfo.InvariantCulture),
                        Engine.GetFunc<T, bool>(xmlModifier.Value, Engine.NewSession), type));
            }
        }
    }
}