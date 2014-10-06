using System;
using System.Linq;

namespace Alpha.Core.Provinces
{
    public class Settlement
    {
        public String Name;

        public Settlement(LandProvince province)
        {
            Name = province.Id + "settlement_" + (province.Settlements.Count() + 1);
        }
    }
}
