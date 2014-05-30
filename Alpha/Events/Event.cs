using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Scripting;

namespace Alpha.Events
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

    class Outcome<T>
    {
        public Func<T, String> LabelFunc { get; set; }
        public Func<T, String> TooltîpFunc { get; set; }
        public Action<T> PreExecute { get; set; }
        public IList<Func<T, Boolean>> Conditions { get; set; }
        public IList<Action<T>> Effects { get; set; }
        public int BaseIaAffinity { get; set; }
        public IList<IModifier<T>> IaAffinityModifiers { get; set; }
        
        public Outcome()
        {
            Conditions = new List<Func<T, bool>>();
            Effects = new List<Action<T>>();
            IaAffinityModifiers = new List<IModifier<T>>();
        }
        public String Label(T item)
        {
            return LabelFunc.Invoke(item);
        }

        public String Tooltip(T item)
        {
            return TooltîpFunc == null ? null : TooltîpFunc.Invoke(item);
        }

        public bool ConditionsValid(T item)
        {
            return Conditions.All(condition => condition.Invoke(item));
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
        public IList<Outcome<T>> Outcomes { get; set; }
        public Action<T> PreExecute { get; set; }

        public Event(string id)
        {
            Conditions = new List<Func<T, bool>>();
            Modifiers = new List<IModifier<T>>();
            Outcomes = new List<Outcome<T>>();
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
            if(!ConditionsValid(item))
                return;
            if (Random.Generator.Get(0, MeanTimeToHappen(item)) == 0)
            {
                if(PreExecute != null)
                    PreExecute.Invoke(item);
                foreach (Outcome<T> outcome in Outcomes)
                {
                    if(!outcome.ConditionsValid(item))
                        continue;
                    Console.WriteLine();
                    if (outcome.PreExecute != null)
                        outcome.PreExecute.Invoke(item);
                    Console.WriteLine(outcome.Label(item) ?? "No label");
                    Console.WriteLine(outcome.Tooltip(item) ?? "No tooltip");
                }
            }
        }
    }
}
