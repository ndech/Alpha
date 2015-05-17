using System;
using System.Data;
using Alpha.Toolkit;

namespace Alpha.Core.Provinces
{
    public class Population : IDailyUpdatableItem
    {
        private double _value = 1000;
        public int LastDayVariation { get; private set; }
        public int Value
        {
            get { return (int)_value; }
            set { _value = Math.Max(0.0f, value + (_value - Value)); }
        }
        public double YearlyGrowth { get; }

        internal Population(int range = 1000)
        {
            _value = RandomGenerator.Get(range/2, 2*range);
            YearlyGrowth = 0;//RandomGenerator.GetDouble(-0.05, 0.05);
        }

        void IDailyUpdatableItem.DayUpdate()
        {
            double population = _value * (1 + (RandomGenerator.GetDouble(YearlyGrowth - 0.5, YearlyGrowth + 0.5) / 365));
            LastDayVariation = (int)population - (int)_value;
            _value = population;
        }
    }
}
