using System;
using System.Collections.Generic;

namespace Alpha.Core.Provinces
{
    class LandProvince : Province
    {
        private float _population = 1000;

        public LandProvince(List<Zone> zones) : base(zones, "land_province"+IdSequence)
        {
            YearlyGrowth = 0.1f;
            Name = "Land";
        }

        public Int32 Population
        {
            get { return (int)_population; }
            set { _population = Math.Max(0.0f, value + (_population - Population)); }
        }
        public float YearlyGrowth { get; private set; }
        public float BaseTax { get { return Population * 0.1f; } }

        protected override void DayUpdate()
        {
            _population += _population * (YearlyGrowth / 365);
        }

        public override sealed string Name { get; internal set; }
    }
}