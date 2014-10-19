using System.Collections.Generic;
using Alpha.WorldGeneration;

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

        public SeaProvince(World world, List<Zone> zones, Cluster cluster) : base(world, zones, cluster)
        {
            Name = "Sea";
        }
    }
}