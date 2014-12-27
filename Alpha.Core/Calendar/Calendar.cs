using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Tags;
using Alpha.Toolkit;

namespace Alpha.Core.Calendar
{
    public class Calendar : Manager, ITagable
    {
        public Season CurrentSeason
        {
            get { return _currentSeason; }
            set
            {
                if(_currentSeason == value) return;
                _currentSeason = value;
                CurrentSeasonLength = 0;
            }
        }
        public int CurrentSeasonLength { get; private set; }
        public Date CurrentDate { get; private set; }
        public TagCollection Tags { get; private set; }

        internal Calendar(World world) : base(world)
        {
            Tags = new TagCollection();
            _seasons = Season.LoadSeasons();
            _months = Month.LoadMonths();
            CurrentDate = new Date(1,_months.Skip(4).First(),1900);
            _currentSeason = _seasons.First();
        }

        internal override void DayUpdate(DataLock dataLock)
        {
            dataLock.Write(() =>
            {
                CurrentDate = NextDay(CurrentDate);
                CurrentSeason = Season.CurrentSeason(_seasons, this);
                CurrentSeasonLength++;
            });
        }

        internal override void Initialize()
        { }

        internal int AgeOf(Date birthdate)
        {
            throw new NotImplementedException();
        }

        private Date NextDay(Date currentDate)
        {
            int lengthOfMonth = currentDate.Month.Length(currentDate.Year);
            int day = currentDate.Day + 1;
            int year = currentDate.Year;
            Month month = currentDate.Month;
            if (day > lengthOfMonth)
            {
                day = 1;
                month = month.Next;
                if (month.Position < currentDate.Month.Position)
                    year++;
            }
            return new Date(day,month,year);
        }

        private Season _currentSeason;
        private readonly IEnumerable<Season> _seasons;
        private readonly IEnumerable<Month> _months;
        public int Year { get { return CurrentDate.Year; } }
        public Month Month { get { return CurrentDate.Month; } }
        public int Day { get { return CurrentDate.Day; } }
    }
}
