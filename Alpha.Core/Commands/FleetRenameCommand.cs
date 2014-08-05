using System;
using Alpha.Core.Fleets;

namespace Alpha.Core.Commands
{
    public class FleetRenameCommand : Command
    {
        private readonly Fleet _fleet;
        private readonly string _newName;

        public FleetRenameCommand(Fleet fleet, String newName)
        {
            _fleet = fleet;
            _newName = newName;
        }

        internal override void Execute()
        {
            _fleet.Name = _newName;
        }
    }
}
