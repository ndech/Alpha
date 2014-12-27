using System;

namespace Alpha.Core.Calendar
{
    public class Date : IEquatable<Date>, IComparable<Date>
    {
        private readonly int _day;
        public int Day { get { return _day; } }

        private readonly Month _month;
        public Month Month { get { return _month; } }

        private readonly int _year;
        public int Year { get { return _year; } }

        public Date(int day, Month month, int year)
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

        public int CompareTo(Date other)
        {
            if (other.Year != Year) return Year - other.Year;
            if (other.Month != Month) return Month.Position - other.Month.Position;
            return Day - other.Day;
        }

        public override String ToString()
        {
            return Day + " " + Month + " " + Year;
        }
    }
}
