using Alpha.Core.Provinces;

namespace Alpha.Core.Commands
{
    class NewConstructionCommand : Command
    {
        private readonly Building _building;
        private readonly Settlement _settlement;

        public NewConstructionCommand(Building building, Settlement settlement)
        {
            _building = building;
            _settlement = settlement;
        }

        internal override bool IsValid()
        {
            return _settlement.Province.Owner.CanAfford(_building.Cost(_settlement));
        }

        internal override void Execute()
        {
            _settlement.StartConstruction(_building);
        }
    }
}
