using System;
using Alpha.Core.Provinces;

namespace Alpha.Core.Commands
{
    public class FoundSettlementCommand : Command
    {
        private readonly LandProvince _province;
        private readonly SettlementType _type;

        public FoundSettlementCommand(LandProvince province, SettlementType type)
        {
            _province = province;
            _type = type;
        }

        internal override void Execute()
        {
            _province.FoundSettlement(_type).Population = 500;
        }

        internal override bool IsValid()
        {
            return _province.Owner.Economy.Treasury >= _type.Cost;
        }
    }
}
