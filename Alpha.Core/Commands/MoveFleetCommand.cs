using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Fleets;
using Alpha.Core.Movement;

namespace Alpha.Core.Commands
{
    public class MoveFleetCommand : Command
    {
        private readonly Fleet _fleet;
        private readonly List<Step> _steps; 
        public MoveFleetCommand(Fleet fleet, List<Step> steps)
        {
            _fleet = fleet;
            _steps = steps;
        }

        internal override void Execute()
        {
            _fleet.MoveOrder = new MoveOrder<Fleet>(_fleet, _steps);
        }

        public override string ToString()
        {
            return "Move fleet " + _fleet + " to " + _steps.Last().Destination + " (" + _steps.Sum(s => s.Duration) + "d)";
        }
    }
}