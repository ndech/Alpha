﻿using Alpha.Toolkit;

namespace Alpha.Core.Calendars
{
    public class Calendar : IManager, IDailyUpdatable
    {
        public Date CurrentDate { get; private set; }

        internal Calendar()
        {
            CurrentDate = new Date(1,1,1900);
        }

        void IDailyUpdatable.DayUpdate(DataLock dataLock)
        {
            dataLock.Write(() => CurrentDate = CurrentDate.NextDay());
        }
    }
}
