namespace Alpha.Core.Provinces
{
    public class Construction : IDailyUpdatableItem
    {
        public Building Building { get; private set; }
        private readonly Settlement _settlement;
        public int RemainingTime { get; set; }
        public Construction(Building building, Settlement settlement)
        {
            Building = building;
            _settlement = settlement;
            RemainingTime = (int)building.ConstructionTime.For(settlement);
        }

        void IDailyUpdatableItem.DayUpdate()
        {
            RemainingTime--;
            if (RemainingTime <= 0)
                _settlement.ConstructionCompleted(this);
        }
    }
}