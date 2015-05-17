using System.Collections.Generic;

namespace Alpha.Core.Provinces
{
    public class SeaProvince : Province
    {
        public SeaProvince(World world, List<Zone> zones) : base(world, zones)
        {
            Name = "Sea";
        }

        public override sealed string Name { get; internal set; }

        protected override string IdPrefix => "sea_province";
    }
}