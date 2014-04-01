using System;
using System.Xml;
using Alpha.Graphics;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha
{

    class Date
    {
        public Int32 Day { get; set; }
        public Int32 Month { get; set; }
        public Int32 Year { get; set; }
    }

    interface ICalendar : IService
    {
         
    }
    class Calendar : GameComponent, ISavable, ICalendar
    {
        public const double TimePerDay = 1.0;
        private double _elapsed = 0;

        public delegate void CalendarChangedEventHandler();
        public event CalendarChangedEventHandler DayChanged;
        public event CalendarChangedEventHandler MonthChanged;
        public event CalendarChangedEventHandler YearChanged;
        public Int32 Day { get; set; }
        public Int32 Month { get; set; }
        public Int32 Year { get; set; }
        public Bool Paused { get; set; }
        public Calendar(IGame game) : base(game, 0)
        {
            Paused = false;
            Day = 1;
            Month = 1;
            Year = 1900;
            DayChanged = () => { };
            MonthChanged = () => { };
            YearChanged = () => { };
        }

        public override void Initialize()
        {
        }

        public override void Update(double delta)
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

        public override void Dispose()
        {
        }

        public override String ToString()
        {
            return Day + @"/" + Month + @"/" + Year + (Paused ? " Paused" : "");
        }

        public void RegisterAsService()
        {
            Game.Services.AddService<ICalendar>(this);
        }

        public int SaveOrder { get; private set; }
        public string SaveName { get; private set; }
        public void Save(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public void PreLoading()
        {
            throw new NotImplementedException();
        }

        public void Load(SaveGame save)
        {
            throw new NotImplementedException();
        }

        public void PostLoading()
        {
            throw new NotImplementedException();
        }
    }
}
