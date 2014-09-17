using Alpha.Toolkit;

namespace Alpha.Core.Calendars
{
    public class Calendar : Manager
    {
        public Date CurrentDate { get; private set; }

        internal Calendar(World world) : base(world)
        {
            CurrentDate = new Date(1,1,1900);
        }

        internal override void DayUpdate(DataLock dataLock)
        {
            dataLock.Write(() => CurrentDate = CurrentDate.NextDay());
        }

        internal override void Initialize()
        { }
    }
}
