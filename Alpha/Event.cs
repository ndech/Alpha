using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using Alpha.Scripting;

namespace Alpha
{
    interface IEvent<T>
    {
        bool IsTriggeredOnly { get; }
    }

    interface IModifier<T>
    {
        double Modifier(T item);

    }

    enum ModifierType
    {
        Multiplier,
        Reducer
    }

    class StaticModifier<T> : IModifier<T>
    {
        private readonly double _factor;
        private readonly Func<T, Boolean> _expression;
        private readonly ModifierType _type;

        public StaticModifier(double factor, Func<T, bool> expression, ModifierType type = ModifierType.Multiplier)
        {
            _factor = factor;
            _expression = expression;
            _type = type;
        }

        double IModifier<T>.Modifier(T item)
        {
            return _expression.Invoke(item) ? _factor * (_type == ModifierType.Reducer ? -1 : 1) : 1;
        }
    }
    class DynamicModifier<T> : IModifier<T>
    {
        private readonly Func<T, Double> _expression;
        private readonly ModifierType _type;

        public DynamicModifier(Func<T, Double> expression, ModifierType type = ModifierType.Multiplier)
        {
            _expression = expression;
            _type = type;
        }

        double IModifier<T>.Modifier(T item)
        {
            return _expression.Invoke(item) * (_type == ModifierType.Reducer ? -1 : 1);
        }
    }

    class Event<T> : IEvent<T> where T : IEventable
    {
        String Id { get; set; }
        public bool IsTriggeredOnly { get; set; }
        public Func<T, String> LabelFunc { get; set; }
        public IList<Func<T, Boolean>> Conditions { get; set; }
        public int BaseMeanTimeToHappen { get; set; }
        public IList<IModifier<T>> Modifiers { get; set; } 


        public Event(string id)
        {
            Conditions = new List<Func<T, bool>>();
            Modifiers = new List<IModifier<T>>();
            Id = id;
        }

        public bool ConditionsValid(T item)
        {
            return Conditions.All(condition => condition.Invoke(item));
        }

        public int MeanTimeToHappen(T item)
        {
            return (int)Modifiers.Aggregate((double)BaseMeanTimeToHappen, (value, multiplier) => value * multiplier.Modifier(item));
        }

        public String Label(T item)
        {
            return LabelFunc.Invoke(item);
        }

        public void Process(T item)
        {
        }
    }
}
