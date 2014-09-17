using System;
using System.Collections.Generic;
using System.Linq;

namespace Alpha.Core.Events
{
    public class Event<T> : IEvent<T> where T : IEventable
    {
        public String Id { get; private set; }
        public bool IsTriggeredOnly { get; set; }
        public void TryTrigger(T eventable)
        {

        }

        private int BaseMeanTimeToHappen { get; set; }
        private IList<IModifier<T>> MeanTimeToHappenModifiers { get; set; }
        private IList<Func<T, Boolean>> PreConditions { get; set; }
        private Func<T, String> LabelGenerator { get; set; }
        public String Label(T item)
        {
            return LabelGenerator(item);
        }

        //        public IList<Action<Object>> Initializers { get; set; } 

        //        public IList<Outcome<T>> Outcomes { get; set; }
        //        public Action<T> PreExecute { get; set; }


        public Event(string id)
        {
            Id = id;
            PreConditions = new List<Func<T, bool>>();
            MeanTimeToHappenModifiers = new List<IModifier<T>>();
            //Outcomes = new List<Outcome<T>>();
            //Initializers = new List<Action<Object>>();
        }

        public int MeanTimeToHappen(T item)
        {
            return (int)MeanTimeToHappenModifiers.Aggregate((double)BaseMeanTimeToHappen, (value, multiplier) => value * multiplier.Modifier(item));
        }

        public bool PreConditionsValid(T item)
        {
            return PreConditions.All(condition => condition(item));
        }
    }
}

//        public void Process(T item)
//        {
//            if(!ConditionsValid(item) || IsTriggeredOnly)
//                return;
//            if (RandomGenerator.Get(0, MeanTimeToHappen(item)) == 0)
//                Execute(item);
//        }

//        public void Execute(IEventable item, object[] parameters)
//        {
//            int parametersCount = (parameters == null ? 0 : parameters.Length);
//            if (Initializers.Count != parametersCount)
//                throw new InvalidEventDataException("Invalid parameter number in event " + Id + " expected " + Initializers.Count + " and got " + parametersCount);
//            if (parametersCount > 0)
//                for (int i = 0; i < Initializers.Count; i++)
//                    Initializers[i](parameters[i]);
//            if (PreExecute != null)
//                PreExecute((T)item);
//            Execute((T)item);
//        }

//        private void Execute(T item)
//        {
//            item.EventResolver.Resolve(item, this);
//        }