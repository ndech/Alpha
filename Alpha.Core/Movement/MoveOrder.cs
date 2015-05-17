using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Provinces;

namespace Alpha.Core.Movement
{
    internal class MoveOrder<T> : IDailyUpdatableItem, IMoveOrder where T : IMovable
    {
        public Zone Destination { get { return _steps.Last().Destination; } }
        private readonly List<Step> _steps;
        public IEnumerable<Step> Steps { get { return _steps; } }
 
        public T Movable { get; }
        public double ProgressToNextStep { get; set; }
        public int CurrentStepIndex { get; set; }
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
                if (CurrentStepIndex == _steps.Count - 1)
                    Movable.EndMovement();
                CurrentStepIndex += 1;
                ProgressToNextStep %= 1;
            }
        }
    }

    public interface IMoveOrder
    {
        Zone Destination { get; }
        IEnumerable<Step> Steps { get; }
        double ProgressToNextStep { get; }
        int CurrentStepIndex { get; }
        Step CurrentStep { get; }
    }
}