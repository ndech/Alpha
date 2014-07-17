using System;
using System.Collections.Generic;
using Alpha.WorldGeneration;

namespace Alpha
{
    class FleetMoveOrder
    {
        public class Step
        {
            public VoronoiSite Destination { get; set; }
            private Int32 Duration { get; set; }

            public Step(VoronoiSite destination, Int32 duration)
            {
                Destination = destination;
                Duration = duration;
            }
        }
        public VoronoiSite Destination { get; set; }
        public List<Step> Steps { get; set; } 
        public Fleet Fleet { get; set; }
        public Int32 Progress { get; set; }

        public FleetMoveOrder(Fleet fleet, VoronoiSite destination, List<Step> steps)
        {
            Progress = 0;
            Fleet = fleet;
            Destination = destination;
            Steps = steps;
        }
    }
}
