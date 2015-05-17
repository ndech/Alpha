using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Commands;
using Alpha.Core.Dynamic;
using Alpha.Toolkit;

namespace Alpha.Core.Events
{
    public class Event<T> : BaseEvent<T> where T : IEventable
    {
        public override bool IsTriggeredOnly { get { return false; } }

        public override IEnumerable<Command> TryTrigger(T eventable)
        {
            if(!IsValid(eventable))
                return null;
            if (RandomGenerator.Get(0, MeanTimeToHappen(eventable)) == 0)
            {
                Console.WriteLine(this + " " + eventable);
                foreach (Outcome<T> outcome in Outcomes)
                {
                    Console.WriteLine(outcome.Label(eventable));
                    return outcome.Execute(eventable);
                }
            }
            return null;
        }

        private Func<T, string> LabelGenerator { get; set; }

        private readonly DynamicValue<T> _meanTimeToHappen;
        public string Label(T item)
        {
            return LabelGenerator(item);
        }

        //        public IList<Action<Object>> Initializers { get; set; } 
        //        public IList<Outcome<T>> Outcomes { get; set; }
        //        public Action<T> PreExecute { get; set; }


        internal Event(string id, IEnumerable<Condition<T>> conditions, DynamicValue<T> meanTimeToHappen, IEnumerable<Outcome<T>> outcomes) : base(id, conditions, outcomes)
        {
            _meanTimeToHappen = meanTimeToHappen;
        }

        public int MeanTimeToHappen(T item)
        {
            return (int) _meanTimeToHappen.For(item);
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