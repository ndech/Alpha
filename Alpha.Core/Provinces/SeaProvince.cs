using System.Collections.Generic;

namespace Alpha.Core.Provinces
{
    class SeaProvince : Province
    {
        public override sealed string Name { get; internal set; }
        protected override void DayUpdate()
        { }

        public SeaProvince(List<Zone> zones) : base(zones, "sea_province_"+IdSequence)
        {
            Name = "Sea";
        }
    }
}