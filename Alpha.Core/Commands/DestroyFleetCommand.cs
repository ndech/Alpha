using Alpha.Core.Fleets;

namespace Alpha.Core.Commands
{
    class DestroyFleetCommand : Command
    {        
        private readonly Fleet _fleet;
        public DestroyFleetCommand(Fleet fleet)
        {
            _fleet = fleet;
        }

        internal override void Execute()
        {
            //_fleet.Destroy();
        }

        public override string ToString()
        {
            return "Destroy fleet " + _fleet;
        }
    }
}
