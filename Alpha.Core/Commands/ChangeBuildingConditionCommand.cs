using Alpha.Core.Buildings;

namespace Alpha.Core.Commands
{
    public class ChangeBuildingConditionCommand : EventOnlyCommand
    {
        private readonly Building _building;
        private readonly double _value;

        public ChangeBuildingConditionCommand(Building building, double value)
        {
            _building = building;
            _value = value;
        }

        internal override void Execute()
        {
            _building.Condition += (float)_value;
        }
    }
}
