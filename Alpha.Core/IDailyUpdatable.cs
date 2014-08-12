using System;

namespace Alpha.Core
{
    internal interface IDailyUpdatable
    {
        void DayUpdate(Object dataLock);
    }
}
