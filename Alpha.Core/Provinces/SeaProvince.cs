using System.Collections.Generic;

namespace Alpha.Core.Provinces
{
    public class SeaProvince : Province
    {
        public override sealed string Name { get; internal set; }
        protected override string GenerateStringId(int id)
        {
            return "sea_province_" + id;
        }

        protected override void DayUpdate()
        { }

        public SeaProvince(World world, List<Zone> zones) : base(world, zones)
        {
            Name = "Sea";
        }
    }
}