using System;

namespace Alpha.Core.Calendar
{
    public class Date : IEquatable<Date>
    {
        private readonly int _day;
        public int Day { get { return _day; } }

        private readonly int _month;
        public int Month { get { return _month; } }

        private readonly int _year;
        public int Year { get { return _year; } }

        public Date(int day, int month, int year)
        {
            _day = day;
            _month = month;
            _year = year;
        }

        public Date(Date date)
        {
            _day = date.Day;
            _month = date.Month;
            _year = date.Year;
        }

        public bool Equals(Date other)
        {
            return (Day == other.Day && Month == other.Month && Year == other.Year);
        }
        public override String ToString()
        {
            return Day + @"/" + Month + @"/" + Year;
        }

        internal Date NextDay()
        {
            int day = Day + 1;
            int month = Month;
            int year = Year;
            if (day > 30)
            {
                day = 1;
                month++;
                if (month > 12)
                {
                    month = 1;
                    year++;
                }
            }
            return new Date(day, month, year);
        }
    }
}
