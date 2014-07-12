using System;
using System.Xml;
using Alpha.Scripting;

namespace Alpha
{

    class Date
    {
        public Int32 Day { get; set; }
        public Int32 Month { get; set; }
        public Int32 Year { get; set; }
    }

    interface ICalendar : IService, IScriptableCalendar
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
            Paused = true;
            Day = 1;
            Month = 1;
            Year = 1900;
        }

        public override void Initialize(Action<string> feedback)
        {
        }

        public override void Update(double delta)
        {
            if (Paused) return;

            _elapsed += delta;
            if (_elapsed > TimePerDay)
            {
                _elapsed = 0;
                Day ++;
                if (Day > 30)
                {
                    Day = 1;
                    Month ++;
                    if (Month > 12)
                    {
                        Month = 1;
                        Year ++;
                        YearChanged.Raise();
                    }
                    MonthChanged.Raise();
                }
                DayChanged.Raise();
            }
        }

        public override void Dispose()
        {
        }

        public override String ToString()
        {
            return Day + @"/" + Month + @"/" + Year;
        }

        public void RegisterAsService()
        {
            Game.Services.Register<ICalendar>(this);
        }

        public int SaveOrder { get { return 0; } }
        public string SaveName { get { return "Calendar"; }}
        public void Save(XmlWriter writer)
        {
            writer.WriteElementString("Day", Day.ToString());
            writer.WriteElementString("Month", Month.ToString());
            writer.WriteElementString("Year", Year.ToString());
        }

        public void PreLoading()
        { }

        public void Load(SaveGame save)
        {
            Paused = true;
            _multiplierIndex = 0;
            _elapsed = 0;
            Day = save.Reader.ReadElementContentAsInt("Day", "");
            Month = save.Reader.ReadElementContentAsInt("Month", "");
            Year = save.Reader.ReadElementContentAsInt("Year", "");
        }

        public void PostLoading()
        { }
    }
}
