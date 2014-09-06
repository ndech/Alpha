using System.Collections.Generic;
using System.Linq;

namespace Alpha
{
    class SeaProvince : Province
    {
        public override sealed string Name { get; set; }
        public SeaProvince(List<Zone> zones) : base(zones, "sea_province_"+IdSequence)
        {
            Name = "Sea";
        }

        public override bool IsCoastalProvince { get { return Adjacencies.Any(a => a.Neighbourg is LandProvince); } }
    }
}
