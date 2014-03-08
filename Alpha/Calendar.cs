using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alpha;
using SharpDX;

namespace PlaneSimulator
{
    class Calendar : IUpdatable
    {
        public const double TimePerDay = 3.0;
        private double _elapsed =0;

        public delegate void DayChangedEventHandler();
        public event DayChangedEventHandler DayChanged;
        public Int32 Day { get; set; }
        public Bool Paused { get; set; }
        public Calendar(Game game)
        {
            Paused = false;
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
            }
        }

        public override String ToString()
        {
            return Day + (Paused ? " Paused" : "");
        }
    }
}
