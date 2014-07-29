using System;

namespace Alpha
{
    abstract class MoveOrder
    {
        public class Step
        {
            public Province Destination { get; private set; }
            public Int32 Duration { get; private set; }

            public Step(Province destination, Int32 duration)
            {
                Destination = destination;
                Duration = duration;
            }
        }
    }
}
