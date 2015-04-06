using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Alpha.Core.Events;
using Alpha.Core.Realms;
using Alpha.Core.Save;
using Alpha.Core.Tags;
using Alpha.Toolkit;

namespace Alpha.Core.Calendars
{
    public class Calendar : Manager, ITagable, IEventable, ISavable
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
            CurrentDate = new Date(28,_months.Skip(11).First(),2014);
            _currentSeason = Season.CurrentSeason(_seasons, this);
        }

        internal override void DayUpdate(DataLock dataLock)
        {
            dataLock.Write(() =>
            {
                CurrentDate = NextDay(CurrentDate);
                CurrentSeason = Season.CurrentSeason(_seasons, this);
                CurrentSeasonLength++;
            });
            Tags.DayUpdate(dataLock);
            TryTriggerEvents(_events, this.Yield(), dataLock);
        }

        internal override void Initialize()
        {
            _events = World.EventManager.LoadEvents<Calendar>();
        }

        internal int AgeOf(Date birthdate)
        {
            return Year - birthdate.Year + ((birthdate.Month.Position < Month.Position || (birthdate.Month == Month && birthdate.Day <= Day)) ? 0 : -1);
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
        private List<IEvent<Calendar>> _events;
        public int Year { get { return CurrentDate.Year; } }
        public Month Month { get { return CurrentDate.Month; } }
        public int Day { get { return CurrentDate.Day; } }

        public Date Date(int day, int month, int year)
        {
            return new Date(day, _months.ElementAt(month-1), year);
        }

        public Realm ResponsibleRealm
        {
            get { throw new System.NotImplementedException(); }
        }
        
        public void Load()
        {
            throw new System.NotImplementedException();
        }

        public XElement Save()
        {
            return new XElement("calendar",
                new XElement("day", Day),
                new XElement("year", Year),
                Tags.Save());
        }
    }
}
