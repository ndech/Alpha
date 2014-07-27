using System.Collections.Generic;

namespace Alpha
{
    class SeaProvince : Province
    {
        public override sealed string Name { get; set; }
        public SeaProvince(List<Zone> zones) : base(zones, "sea_province_"+IdSequence)
        {
            Name = "Sea";
        }
    }
}
