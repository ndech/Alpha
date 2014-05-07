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
        bool Pause();
        bool Unpause();
        void DecreaseMultiplier();
        void IncreaseMultiplier();
        int Multiplier { get; }
        bool Paused { get; set; }
    }
    class Calendar : GameComponent, ISavable, ICalendar
    {
        private static readonly int[] AvailableMultipliers = new[] {1, 2, 3, 5, 10};
        private int _multiplierIndex = 0;

        public void IncreaseMultiplier()
        {
            if (_multiplierIndex < AvailableMultipliers.Length - 1)
                _multiplierIndex++;
        }
        public void DecreaseMultiplier()
        {

            if (_multiplierIndex > 0)
                _multiplierIndex--;
        }
        public int Multiplier { get { return AvailableMultipliers[_multiplierIndex]; } }
        public double TimePerDay {get { return 5.0f / Multiplier; }}
        private double _elapsed = 0;

        public event CustomEventHandler DayChanged;
        public event CustomEventHandler MonthChanged;
        public event CustomEventHandler YearChanged;
        public Int32 Day { get; set; }
        public Int32 Month { get; set; }
        public Int32 Year { get; set; }
        public bool Paused { get; set; }

        public bool Pause()
        {
            bool previousStatus = Paused;
            Paused = true;
            return previousStatus;
        }

        public bool Unpause()
        {
            bool previousStatus = Paused;
            Paused = false;
            return previousStatus;
        }

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
