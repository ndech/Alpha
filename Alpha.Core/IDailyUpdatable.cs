using System.Collections.Generic;
using Alpha.Toolkit;

namespace Alpha.Core
{
    interface IDailyUpdatable
    {
        void DayUpdate(DataLock dataLock);
    }
}
