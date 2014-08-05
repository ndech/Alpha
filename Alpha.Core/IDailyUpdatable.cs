using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha.Core
{
    internal interface IDailyUpdatable
    {
        void DayUpdate(Object dataLock);
    }
}
