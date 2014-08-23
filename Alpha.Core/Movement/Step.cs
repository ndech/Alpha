using System;
using Alpha.Core.Provinces;

namespace Alpha.Core.Movement
{
    public class Step
    {
        public Province Destination { get; private set; }
        public Int32 Duration { get; private set; }

        internal Step(Province destination, Int32 duration)
        {
            Destination = destination;
            Duration = duration;
        }
    }
}