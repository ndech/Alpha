using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Scripting;

namespace Alpha.Events
{

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
            double value = _expression.Invoke(item) ? _factor : 1.0;
            return (_type == ModifierType.Reducer ? 1.0 / value : value);
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
            double value = _expression.Invoke(item);
            return (_type == ModifierType.Reducer ? 1.0/value : value);
        }
    }

    class Event<T> : IEvent<T> where T : IEventable
    {
        public String Id { get; private set; }
        public bool IsTriggeredOnly { get; set; }
        public Func<T, String> LabelFunc { get; set; }
        public IList<Func<T, Boolean>> Conditions { get; set; }
        public IList<Action<Object>> Initializers { get; set; } 
        public int BaseMeanTimeToHappen { get; set; }
        public IList<IModifier<T>> Modifiers { get; set; }
        public IList<Outcome<T>> Outcomes { get; set; }
        public Action<T> PreExecute { get; set; }

        public Event(string id)
        {
            Conditions = new List<Func<T, bool>>();
            Modifiers = new List<IModifier<T>>();
            Outcomes = new List<Outcome<T>>();
            Initializers = new List<Action<Object>>();
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
            if(!ConditionsValid(item) || IsTriggeredOnly)
                return;
            if (Random.Generator.Get(0, MeanTimeToHappen(item)) == 0)
                Execute(item);
        }
        public void Execute(IEventable item, object[] parameters)
        {
            int parametersCount = (parameters == null ? 0 : parameters.Length);
            if (Initializers.Count != parametersCount)
                throw new InvalidEventDataException("Invalid parameter number in event " + Id + " expected " + Initializers.Count + " and got " + parametersCount);
            if (parametersCount > 0)
                for (int i = 0; i < Initializers.Count; i++)
                    Initializers[i].Invoke(parameters[i]);
            if (PreExecute != null)
                PreExecute.Invoke((T)item);
            Execute((T)item);
        }
        private void Execute(T item)
        {
            item.EventResolver.Resolve<T>(item, this);
        }
    }
}
