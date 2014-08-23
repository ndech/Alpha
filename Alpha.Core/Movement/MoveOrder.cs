using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Provinces;

namespace Alpha.Core.Movement
{
    internal class MoveOrder<T> : IDailyUpdatableItem where T : IMovable
    {
        public Province Destination { get { return Steps.Last().Destination; } }
        public List<Step> Steps { get; set; } 
        public T Movable { get; set; }
        public Int32 CurrentStepProgress { get; set; }
        public Int32 ElapsedProgress { get; set; }
        
        public MoveOrder(T item, List<Step> steps)
        {
            Movable = item;
            Steps = steps;
        }

        void IDailyUpdatableItem.DayUpdate()
        {
            CurrentStepProgress++;
            if (CurrentStepProgress >= Steps[0].Duration)
            {
                Movable.SetLocation(Steps[0].Destination);
                ElapsedProgress += CurrentStepProgress;
                Steps.RemoveAt(0);
                CurrentStepProgress = 0;
            }
        }
    }
}