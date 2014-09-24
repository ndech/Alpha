﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Alpha.Core.Events
{
    public class Outcome<T>
    {
        public Func<T, String> LabelFunc { private get; set; }
        public Func<T, String> TooltîpFunc { private get; set; }
        public Action<T> PreExecute { get; set; }
        public IList<Func<T, Boolean>> Conditions { private get; set; }
        public IList<Action<T>> Effects { get; set; }
        public int BaseIaAffinity { get; set; }
        private IList<IModifier<T>> IaAffinityModifiers { get; set; }

        public Outcome()
        {
            Conditions = new List<Func<T, bool>>();
            Effects = new List<Action<T>>();
            IaAffinityModifiers = new List<IModifier<T>>();
        }

        public String Label(T item)
        {
            return LabelFunc(item);
        }

        public String Tooltip(T item)
        {
            return TooltîpFunc == null ? null : TooltîpFunc(item);
        }

        public bool ConditionsValid(T item)
        {
            return Conditions.All(condition => condition(item));
        }

        public Double IaAffinity(T item)
        {
            return IaAffinityModifiers.Aggregate((double)BaseIaAffinity, (value, multiplier) => value * multiplier.Modifier(item));
        }
    }
}