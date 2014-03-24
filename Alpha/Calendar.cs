using System;
using Alpha;
using SharpDX;

namespace PlaneSimulator
{
    class Calendar : IUpdatable
    {
        public const double TimePerDay = 1.0;
        private double _elapsed =0;

        public delegate void CalendarChangedEventHandler();
        public event CalendarChangedEventHandler DayChanged;
        public event CalendarChangedEventHandler MonthChanged;
        public event CalendarChangedEventHandler YearChanged;
        public Int32 Day { get; set; }
        public Int32 Month { get; set; }
        public Int32 Year { get; set; }
        public Bool Paused { get; set; }
        public Calendar(Game game)
        {
            Paused = false;
            Day = 1;
            Month = 1;
            Year = 1900;
            DayChanged = () => { };
            MonthChanged = () => { };
            YearChanged = () => { };
        }

        public void Update(double delta)
        {
            if (Paused) return;
            _elapsed += delta;
            if (_elapsed > TimePerDay)
            {
                DayChanged.Invoke();
                _elapsed = 0;
                Day ++;
                if (Day > 30)
                {
                    Day = 1;
                    Month ++;
                    MonthChanged.Invoke();
                    if (Month > 12)
                    {
                        Month = 1;
                        Year ++;
                        YearChanged.Invoke();
                    }
                }
            }
        }

        public override String ToString()
        {
            return Day + @"/" + Month + @"/" + Year + (Paused ? " Paused" : "");
        }
    }
}
