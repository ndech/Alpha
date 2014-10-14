namespace Alpha.Core.Provinces
{
    public class Construction : IDailyUpdatableItem
    {
        public Building Building { get; private set; }
        private readonly Settlement _settlement;
        public int ConstructionDuration { get; private set; }
        public int Elapsed { get; set; }
        public Construction(Building building, Settlement settlement)
        {
            Building = building;
            _settlement = settlement;
            ConstructionDuration = building.ConstructionDuration(settlement);
            Elapsed = 0;
        }

        void IDailyUpdatableItem.DayUpdate()
        {
            Elapsed++;
            if (Elapsed >= ConstructionDuration)
                _settlement.ConstructionCompleted(this);
        }
    }
}