using System;

namespace Alpha.Core.Calendar
{
    public class Season
    {
        public String Id { get; private set; }
        public String Name { get; private set; }
        internal Func<Calendar, bool> Trigger { get; private set; }

        public Season(String id, String name, Func<Calendar, bool> trigger)
        {
            Id = id;
            Name = name;
            Trigger = trigger;
        }
    }
}
