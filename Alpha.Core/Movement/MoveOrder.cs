using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Provinces;

namespace Alpha.Core.Movement
{
    internal class MoveOrder<T> : IDailyUpdatableItem, IMoveOrder where T : IMovable
    {
        public Province Destination { get { return _steps.Last().Destination; } }
        private readonly List<Step> _steps;
        public IEnumerable<Step> Steps { get { return _steps; } }
 
        public T Movable { get; private set; }
        public Double ProgressToNextStep { get; set; }
        public Int32 CurrentStepIndex { get; set; }
        public Step CurrentStep { get { return _steps[CurrentStepIndex]; } }
        
        public MoveOrder(T item, List<Step> steps)
        {
            Movable = item;
            _steps = steps;
            ProgressToNextStep = 0.0f;
            CurrentStepIndex = 0;
        }

        void IDailyUpdatableItem.DayUpdate()
        {
            ProgressToNextStep += 1.0f/_steps[CurrentStepIndex].Duration(Movable.Speed);
            if (ProgressToNextStep >= 1.0f)
            {
                Movable.SetLocation(_steps[CurrentStepIndex].Destination);
                CurrentStepIndex += 1;
                ProgressToNextStep %= 1;
            }
        }
    }

    public interface IMoveOrder
    {
        Province Destination { get; }
        IEnumerable<Step> Steps { get; }
        Double ProgressToNextStep { get; }
        Int32 CurrentStepIndex { get; }
        Step CurrentStep { get; }
    }
}