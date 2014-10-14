using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Alpha.Core.Dynamic
{
    public class DynamicValue<T>
    {
        private readonly List<IModifier<T>> _modifiers;
        private readonly Double _base;

        public double For(T item)
        {
            return _modifiers.Aggregate(_base, (value, multiplier) => value * multiplier.Modifier(item));
        }
        
        internal DynamicValue(Double @base, List<IModifier<T>> modifiers)
        {
            _base = @base;
            _modifiers = modifiers;
        }

        internal DynamicValue(XElement definition)
        {
            _base = Double.Parse(definition.Element("base").Value, CultureInfo.InvariantCulture);
            _modifiers = new List<IModifier<T>>();
            XElement xModifiers = definition.Element("modifiers");
            String scriptIdentifier = typeof(T).Name;
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
                        Engine.Execute<Func<T, Double>>("(" + scriptIdentifier + ") => " + xmlModifier.Value, Engine.NewSession), type));
                else
                    _modifiers.Add(new StaticModifier<T>(Double.Parse(xmlModifier.Attribute("factor").Value),
                        Engine.Execute<Func<T, Boolean>>("(" + scriptIdentifier + ") => " + xmlModifier.Value, Engine.NewSession), type));
            }
        }
    }
}