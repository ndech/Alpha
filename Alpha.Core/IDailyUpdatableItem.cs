using System.Collections.Generic;
using Alpha.Toolkit;

namespace Alpha.Core
{
    interface IDailyUpdatableItem
    {
        void DayUpdate();
    }

    internal static class DailyUpdatableItemExtension
    {
        public static void DayUpdate(this IEnumerable<IDailyUpdatableItem> items)
        {
            foreach (IDailyUpdatableItem item in items)
                item.DayUpdate();
        }

        public static void DayUpdate(this IDailyUpdatableItem item)
        {
            item.DayUpdate();
        }

        public static void DayUpdate(this IEnumerable<IDailyUpdatableItem> items, DataLock dataLock)
        {
            foreach (IDailyUpdatableItem item in items)
                dataLock.Write(item.DayUpdate);
        }
    }
}
