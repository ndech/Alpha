using System;
using System.Xml;
using SharpDX;

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
        event CustomEventHandler DayChanged;
    }
    class Calendar : GameComponent, ISavable, ICalendar
    {
        public const double TimePerDay = 1.0;
        private double _elapsed = 0;

        public event CustomEventHandler DayChanged;
        public event CustomEventHandler MonthChanged;
        public event CustomEventHandler YearChanged;
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
                DayChanged.Raise();
                _elapsed = 0;
                Day ++;
                if (Day > 30)
                {
                    Day = 1;
                    Month ++;
                    MonthChanged.Raise();
                    if (Month > 12)
                    {
                        Month = 1;
                        Year ++;
                        YearChanged.Raise();
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
