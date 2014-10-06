using System;
using System.Linq;

namespace Alpha.Core.Provinces
{
    public class Settlement
    {
        public String Name { get; private set; }
        public LandProvince Province { get; private set; }

        public Settlement(LandProvince province)
        {
            Province = province;
            Name = province.Id + "settlement_" + (province.Settlements.Count() + 1);
        }
    }
}
