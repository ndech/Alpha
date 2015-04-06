using Alpha.Core.Buildings;
using Alpha.Core.Provinces;

namespace Alpha.Core.Commands
{
    class NewConstructionCommand : Command
    {
        private readonly BuildingType _buildingtype;
        private readonly Settlement _settlement;

        public NewConstructionCommand(BuildingType buildingtype, Settlement settlement)
        {
            _buildingtype = buildingtype;
            _settlement = settlement;
        }

        internal override bool IsValid()
        {
            return true;
        }

        internal override void Execute()
        {
            _settlement.StartConstruction(_buildingtype);
        }
    }
}
