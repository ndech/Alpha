namespace Alpha.Core.Calendars
{
    public class Calendar : IManager, IDailyUpdatable
    {
        public Date CurrentDate { get; private set; }

        internal Calendar()
        {
            CurrentDate = new Date(1,1,1900);
        }

        void IDailyUpdatable.DayUpdate(object dataLock)
        {
            CurrentDate = CurrentDate.NextDay();
        }

        void IManager.Setup()
        {
            
        }
    }
}
