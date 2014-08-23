namespace Alpha.Core.Realms
{
    public class RealmEconomy : IDailyUpdatableItem
    {
        public double Treasury { get; internal set; }
        internal RealmEconomy()
        {
            Treasury = 100;
        }

        void IDailyUpdatableItem.DayUpdate()
        {

        }
    }
}
