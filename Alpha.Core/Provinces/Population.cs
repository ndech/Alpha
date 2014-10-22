using System;
using System.Data;
using Alpha.Toolkit;

namespace Alpha.Core.Provinces
{
    public class Population : IDailyUpdatableItem
    {

        private double _value = 1000;
        public int LastDayVariation { get; private set; }
        public Int32 Value
        {
            get { return (int)_value; }
            set { _value = Math.Max(0.0f, value + (_value - Value)); }
        }
        public double YearlyGrowth { get; private set; }

        internal Population(int range = 1000)
        {
            _value = RandomGenerator.Get(range/2, 2*range);
            YearlyGrowth = RandomGenerator.GetDouble(-0.3, 0.9);
        }

        void IDailyUpdatableItem.DayUpdate()
        {
            double population = _value * (1 + (RandomGenerator.GetDouble(YearlyGrowth - 0.5, YearlyGrowth + 0.5) / 365));
            LastDayVariation = (int)population - (int)_value;
            _value = population;
        }
    }
}
