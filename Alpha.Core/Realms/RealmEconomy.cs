namespace Alpha.Core.Realms
{
    public class RealmEconomy : IDailyUpdatable
    {
        public double Treasury { get; internal set; }
        internal RealmEconomy()
        {
            Treasury = 100;
        }

        void IDailyUpdatable.DayUpdate(object dataLock)
        {

        }
    }
}
