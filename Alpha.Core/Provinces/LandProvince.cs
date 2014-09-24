using System;
using System.Collections.Generic;
using Alpha.Core.Realms;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;

namespace Alpha.Core.Provinces
{
    public class LandProvince : Province
    {
        private float _population = 1000;

        public LandProvince(List<Zone> zones) : base(zones)
        {
            YearlyGrowth = 0.1f;
            Name = "Land";
            Color = new Tuple<float, float, float>(
                (float)RandomGenerator.GetDouble(0, 1), 
                (float)RandomGenerator.GetDouble(0, 1),
                (float)RandomGenerator.GetDouble(0, 1));
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
        protected override string GenerateStringId(int id)
        {
            return "land_province" + id;
        }

        public Realm Owner { get; internal set; }
        public Tuple<float, float, float> Color { get; internal set; }
    }
}